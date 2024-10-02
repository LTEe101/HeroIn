using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static CameraController;
using static Outline;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PhotonPlayerController: MonoBehaviour
{
    public static PhotonPlayerController Instance { get; private set; }

    Animator anim;
    private Rigidbody rb;
    public float jumpForce = 7f; // 점프 힘
    private Vector3 moveDirection; // 이동 방향
    public LayerMask groundLayer; // 바닥 레이어 설정 (점프할 수 있는 곳)
    private bool isGrounded; // 캐릭터가 바닥에 있는지 여부
    float _speed = 7.0f;
    Vector3 _destPos;
    float _mouseSensitivity = 2.0f; // 마우스 감도
    private PlayerInput playerInput; // PlayerInput 변수 선언

    // 상호 작용
    [SerializeField] private TMP_Text _interactText; // 상호작용 가능할 때 나타나는 텍스트 UI
    [SerializeField] private float _checkRate = 0.05f; // 광선을 쏘는 주기 (0.05초마다)
    [SerializeField] private float _maxDistance = 3.0f; // 광선이 감지할 수 있는 최대 거리
    [SerializeField] private LayerMask _layerMask; // 상호작용할 수 있는 오브젝트를 감지할 레이어
    private float _lastCheckTime; // 마지막으로 광선을 쏜 시간을 기록
    private GameObject _curGameobject; // 현재 감지된 오브젝트
    private IInteractable _curInteractable; // 현재 상호작용 가능한 인터페이스를 구현한 오브젝트

    // 카메라
    public Transform cameraTransform; // 카메라의 Transform
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f); // 초기 위치
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전
    public Vector3 cameraOffset = new Vector3(0, 7.7f, -7); // 카메라가 캐릭터로부터 떨어진 거리
    private float xRotation = 0f; // 카메라 회전 속도 설정

    // 포톤
    public PhotonView PV;
    private GameObject chatBubblePrefab; // 말풍선 UI 프리팹을 연결할 변수
    private GameObject currentChatBubble; // 현재 말풍선 인스턴스를 저장할 변수

    public TMP_Text playerNameText; // 캐릭터 이름 표시용 TextMeshPro
    public string playerName; // 플레이어 이름

    public enum PlayerState
    {
        Chatting,
        Jumping,
        Moving,
        Idle,
        Watch,
    }

    public PlayerState State { get; private set; } = PlayerState.Idle;

    public void SetState(PlayerState state)
    {
        State = state;
    }

    void Start()
    {
        chatBubblePrefab = Resources.Load<GameObject>("Prefabs/ChatBubble");
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        anim = GetComponent<Animator>(); // 애니메이션 컴포넌트
        PV = GetComponent<PhotonView>(); // PhotonView 초기화
        _destPos = transform.position;  // 시작 위치 설정
        cameraTransform = Camera.main.transform; // 기본 메인 카메라를 설정
        _layerMask = LayerMask.GetMask("Interactable"); // "Interactable" 레이어에 속하는 오브젝트만 감지하도록 설정
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Interaction"].performed += OnInteraction;
        UpdateCameraPosition();

        // 만약 이 플레이어가 로컬 플레이어라면 ChatManager에 등록
        if (PV.IsMine)
        {
            ChatManager.Instance.SetLocalPlayer(this);
            if (Managers.Data.userInfo != null)
            {
                playerName = Managers.Data.userInfo.name;
            }
            else
            {
                playerName = "나의이름은";
            }
           
        }
        playerNameText.text = playerName; // 이름 표시
                                          
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 잠금 해제
    }

    void UpdateJumping()
    {
        // 점프 중 이동 처리
        if (moveDirection != Vector3.zero)
        {
            rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
        }
        if (IsGrounded()) // 착지했을 때 상태를 Idle로 전환
        {
            State = PlayerState.Idle;
            anim.SetBool("isJumping", false);
        }
    }

    void UpdateMoving()
    {
        // 플레이어 전방에 벽이 있는지 레이캐스트로 감지
        Vector3 dir = moveDirection; // 이동 방향
        RaycastHit hit;
        float rayDistance = 1.0f; // 레이캐스트 거리 설정
        LayerMask wallLayer = LayerMask.GetMask("Wall"); // Wall 레이어 설정

        // 플레이어 전방으로 레이캐스트를 쏘아 벽 감지
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, dir.normalized, out hit, rayDistance, wallLayer))
        {
            Debug.Log("벽과 충돌: " + hit.collider.name);
            State = PlayerState.Idle; // 벽에 부딪히면 이동 멈춤
            return;
        }

        RayInteractable();
         // 이동 중이 아닌 경우 Idle 상태로 전환
        if (moveDirection == Vector3.zero)
        {
            State = PlayerState.Idle;
        }
        else
        {
            // 이동 처리
            rb.MovePosition(transform.position + moveDirection * Time.deltaTime);

            // 애니메이션 설정
            anim.SetFloat("speed", _speed);
        }
    }

    void UpdateIdle()
    {
        // 이동 입력이 없을 때 Idle 상태
        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        // 이름 텍스트가 존재할 때, 항상 카메라를 바라보게 설정
        if (playerNameText != null)
        {
            playerNameText.transform.LookAt(Camera.main.transform);
            playerNameText.transform.rotation = Quaternion.Euler(0, playerNameText.transform.rotation.eulerAngles.y + 180, 0); // 180도 회전
        }

        // 말풍선이 존재할 때, 매 프레임마다 카메라를 바라보게 회전시킴
        if (currentChatBubble != null)
        {
            // 모든 캐릭터의 말풍선을 자신의 카메라 기준으로 회전시킴
            currentChatBubble.transform.LookAt(Camera.main.transform);

            // 말풍선이 카메라를 정확히 바라보게 하도록 Y축을 180도 돌려줌 (필요에 따라 조정 가능)
            currentChatBubble.transform.rotation = Quaternion.Euler(0, currentChatBubble.transform.rotation.eulerAngles.y + 180, 0);
        }
        if (PV.IsMine)
        {
            // Chatting 상태일 때는 아무 동작도 하지 않음
            if (State == PlayerState.Chatting)
            {
                
                if (IsGrounded()) 
                {
                    anim.SetFloat("speed", 0); // 이동 애니메이션 중지
                    anim.SetBool("isJumping", false);
                }
                return;
            }
            // ESC 키를 누르면 Watch 상태에서 나와서 Idle 상태로 전환
            if (Input.GetKeyDown(KeyCode.Escape) && State == PlayerState.Watch)
            {
                ExitWatchState();
            }

            if (State == PlayerState.Watch) return;

            switch (State)
            {
                case PlayerState.Jumping:
                    UpdateJumping();
                    break;
                case PlayerState.Moving:
                    UpdateMoving();
                    break;
                case PlayerState.Idle:
                    UpdateIdle();
                    break;
            }

            // 카메라
            HandleMouseRotation();
            UpdateCameraPosition();
            OnKeyboard();
        }
    }
    void RayInteractable()
    {
        // 마지막 광선 발사 시간과 현재 시간을 비교하여 체크 주기(_checkRate) 이상일 때만 수행
        if (Time.time - _lastCheckTime > _checkRate)
        {
            _lastCheckTime = Time.time; // 현재 시간을 마지막 체크 시간으로 업데이트

            // 화면 중심에서 광선을 쏨 (화면의 중간 좌표 사용)
            Ray ray = new Ray(transform.position + new Vector3(0, 1.5f, 0), transform.forward);
            RaycastHit hit;
            Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), Vector3.forward * 5.0f, Color.red); // 5.0f 길이로 앞으로 그리기
            // 광선이 오브젝트와 충돌했는지, 그리고 그 오브젝트가 설정한 레이어에 속하는지 확인
            if (Physics.Raycast(ray, out hit, _maxDistance, _layerMask))
            {
                // 새로 감지된 오브젝트가 이전에 감지된 오브젝트와 다를 때만 실행
                if (hit.collider.gameObject != _curGameobject)
                {
                    _curGameobject = hit.collider.gameObject; // 현재 감지된 오브젝트 저장
                    ChangeOutlineColor(_curGameobject, Color.white);
                    _curInteractable = hit.collider.GetComponent<IInteractable>(); // IInteractable 인터페이스 구현 확인
                    SetPromptText(); // UI 텍스트 설정 함수 호출
                }
            }
            else
            {
                ChangeOutlineColor(_curGameobject, Color.black);
                // 감지된 오브젝트가 없을 때 처리 (텍스트 비활성화 및 오브젝트 정보 초기화)
                _curGameobject = null;
                _curInteractable = null;
                _interactText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화
            }
        }
    }
    void ChangeOutlineColor(GameObject go, Color newColor)
    {
        if (go == null) return;
        // 오브젝트에서 Outline 컴포넌트를 가져옴
        Outline outline = go.GetComponent<Outline>();

        // Outline 컴포넌트가 있으면 색상 변경
        if (outline != null)
        {
            // 아웃라인 컬러 변경
            outline.OutlineColor = newColor;
        }
        else
        {
            Debug.LogWarning("Outline 컴포넌트를 찾을 수 없습니다.");
        }
    }

    // UI 텍스트를 활성화하고, 상호작용 가능한 오브젝트의 상호작용 텍스트를 표시
    private void SetPromptText()
    {
        _interactText.gameObject.SetActive(true); // 텍스트 UI를 활성화
        _interactText.text = _curInteractable.GetInteractText(); // 현재 상호작용 가능한 오브젝트의 상호작용 텍스트를 가져와서 표시
    }

    // 상호작용 입력 처리 (Input 시스템에서 호출됨)
    public void OnInteraction(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("OnInteract"); // 상호작용 시 디버그 메시지 출력
        Debug.Log($"OnInteract, {callbackContext.phase} {_curInteractable}"); // 상호작용 상태와 현재 상호작용 가능한 오브젝트 정보 출력

        // 상호작용 입력이 시작되고, 상호작용 가능한 오브젝트가 존재할 때
        if (callbackContext.phase == InputActionPhase.Performed && _curInteractable != null)
        {
            if (State == PlayerState.Watch)
            {
                ExitWatchState(); // Watch 상태에서 나옴
                UpdateCameraPosition();
                SetRenderersEnabled(true);
            }
            else if (_curInteractable != null)
            {
                Debug.Log("상호작용 중...");
                State = PlayerState.Watch; // Watch 상태로 진입
                _curInteractable.Interact(); // 상호작용 로직 실행
                cameraTransform.position = _initialPosition;
                cameraTransform.rotation = _initialRotation;
                _interactText.gameObject.SetActive(false); // 텍스트 비활성화
                UpdateIdle();
                SetRenderersEnabled(false);
            }
            //_curGameobject = null; // 상호작용 후 오브젝트 정보를 초기화 (현재는 주석 처리됨)
            //_curInteractable = null; // 상호작용 가능한 오브젝트 정보도 초기화 (현재는 주석 처리됨)
        }
    }

    // Watch 상태에서 나오는 함수
    void ExitWatchState()
    {
        Debug.Log("Watch 상태 종료, Idle 상태로 전환");
        State = PlayerState.Idle; // 상태를 Idle로 전환
    }


    void SetRenderersEnabled(bool isEnabled)
    {
        // 해당 게임 오브젝트와 그 하위 모든 오브젝트의 Renderer를 찾아서 설정
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isEnabled;
        }
    }
    // 채팅
    [PunRPC]
    public void ShowChatBubble(string message)
    {
        if (currentChatBubble != null)
        {
            Destroy(currentChatBubble); // 기존 말풍선이 있으면 삭제
        }
        Debug.Log(message);

        // 말풍선을 캐릭터의 머리 위 고정된 위치로 생성 (캐릭터 크기에 따라 적절히 조정)
        Vector3 bubblePosition = transform.position + Vector3.up * 3.5f; // 머리 위로 고정

        // 말풍선을 캐릭터의 머리 위 고정 위치로 생성
        currentChatBubble = Instantiate(chatBubblePrefab, bubblePosition, Quaternion.identity);

        // 부모를 캐릭터로 설정하여 캐릭터와 함께 움직이도록 설정
        currentChatBubble.transform.SetParent(transform);

        TMP_Text chatText = currentChatBubble.GetComponentInChildren<TMP_Text>();
        chatText.text = message;

        // 일정 시간 후 말풍선을 자동으로 제거
        Destroy(currentChatBubble, 5f); // 5초 후 삭제
    }

    // 키 감지하는 함수
    void OnKeyboard()
    {
        if (State == PlayerState.Jumping) return;

        // 이동 입력 처리
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 카메라 방향을 기준으로 이동 방향 설정
        Vector3 forward = cameraTransform.forward; // 카메라의 앞방향
        Vector3 right = cameraTransform.right;     // 카메라의 오른쪽방향

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveZ + right * moveX).normalized * _speed;

        if (moveDirection != Vector3.zero)
        {
            State = PlayerState.Moving;
        }

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }
    }

    // 점프 함수
    private void Jump()
    {

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        State = PlayerState.Jumping;
        anim.SetBool("isJumping", true);
    }


    // 땅에 있는지 확인하는 함수
    private bool IsGrounded()
    {
        // 캐릭터의 아래로 스피어캐스트를 쏴서 바닥에 있는지 확인
        RaycastHit hit;
        float distanceToGround = 0.5f; // 바닥과의 최소 거리
        float sphereRadius = 0.3f; // 스피어의 반경 (캐릭터 크기에 맞게 조정)
        Vector3 origin = transform.position + Vector3.up * 0.7f; // 캐릭터 중심에서 약간 위에서 시작

        // 디버그용 스피어캐스트 그리기 (시각적으로 확인)
        Debug.DrawRay(origin, Vector3.down * (distanceToGround + sphereRadius), Color.red);

        // 스피어캐스트를 사용하여 바닥 감지
        if (Physics.SphereCast(origin, sphereRadius, Vector3.down, out hit, distanceToGround, groundLayer))
        {
            return true; // 바닥에 닿아 있으면 true 반환
        }

        return false; // 공중에 있으면 false 반환
    }


    // 카메라
    void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1)) // 마우스 우클릭 시
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // 캐릭터와 카메라 Y축 회전
            transform.Rotate(0, mouseX, 0); // 캐릭터 회전

            // 마우스 상하 움직임에 따른 카메라 오프셋 조절 (카메라 높이 조절)
            cameraOffset.y -= mouseY * 0.1f; // 감도를 조정해 자연스러운 높이 변화
            cameraOffset.y = Mathf.Clamp(cameraOffset.y, 1f, 10f); // 카메라 높이 제한 (1 ~ 10)

            // 카메라 상하 각도 조절
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -45f, 45f); // 카메라 상하 각도를 제한 (-45도 ~ 45도)

            // 카메라 상하 회전 적용
            cameraTransform.localRotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
        }
    }

    // 카메라가 캐릭터를 따라다니도록 위치 업데이트
    private void UpdateCameraPosition()
    {
        if (cameraTransform != null)
        {
            // 캐릭터의 회전을 반영하여 카메라 위치 설정
            Vector3 targetPosition = transform.position + transform.rotation * cameraOffset;
            cameraTransform.position = targetPosition;

            // 카메라가 캐릭터를 바라보도록 설정
            cameraTransform.LookAt(transform.position + Vector3.up * 3f);
        }
    }
}
