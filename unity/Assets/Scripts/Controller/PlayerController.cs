using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static CameraController;
using static Outline;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    // 이동
    Animator anim;
    private Rigidbody rb;
    float _speed = 5.0f;
    private Vector3 moveDirection; // 이동 방향
    Vector3 _destPos;
    NavMeshAgent nav;

    // 카메라
    public Transform cameraTransform; // 카메라의 Transform
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f); // 초기 위치
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전
    public Vector3 cameraOffset = new Vector3(0, 7.7f, -7); // 카메라가 캐릭터로부터 떨어진 거리
    private float xRotation = 0f; // 카메라 회전 속도 설정
    float _mouseSensitivity = 2.0f; // 마우스 감도

    // 상호 작용
    [SerializeField] private TMP_Text _interactText; // 상호작용 가능할 때 나타나는 텍스트 UI
    private float _checkRate = 0.05f; // 광선을 쏘는 주기 (0.05초마다)
    private float _maxDistance = 3.0f; // 광선이 감지할 수 있는 최대 거리
    private LayerMask _layerMask; // 상호작용할 수 있는 오브젝트를 감지할 레이어
    private float _lastCheckTime; // 마지막으로 광선을 쏜 시간을 기록
    private GameObject _curGameobject; // 현재 감지된 오브젝트
    private IInteractable _curInteractable; // 현재 상호작용 가능한 인터페이스를 구현한 오브젝트
    private PlayerInput playerInput; // PlayerInput 변수 선언

    public enum PlayerState
    {
        Moving,
        Idle,
        Watch,
    }
    public PlayerState State { get; private set; } = PlayerState.Idle;
    public void SetState(PlayerState state)
    {
        State = state;
    }
    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // 캐릭터
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        anim = GetComponent<Animator>(); // 애니메이션 컴포넌트
        nav = GetComponent<NavMeshAgent>();
        _destPos = transform.position;  // 시작 위치 설정

        // 상호작용
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Interaction"].performed += OnInteraction;
        _layerMask = LayerMask.GetMask("Interactable"); // "Interactable" 레이어에 속하는 오브젝트만 감지하도록 설정

        // 카메라
        cameraTransform = Camera.main.transform; // 기본 메인 카메라를 설정
        UpdateCameraPosition();
    }
   
    void UpdateMoving()
    {

        RayInteractable();
        // 이동 중이 아닌 경우 Idle 상태로 전환
        if (moveDirection == Vector3.zero)
        {
            State = PlayerState.Idle;
        }
        else
        {
            // 이동 처리
            nav.Move(moveDirection * Time.deltaTime);

            // 애니메이션 설정
            anim.SetFloat("speed", _speed);
        }
    }

    void UpdateIdle()
    {
        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        // ESC 키를 누르면 Watch 상태에서 나와서 Idle 상태로 전환
        if (Input.GetKeyDown(KeyCode.Escape) && State == PlayerState.Watch)
        {
            ExitWatchState();
        }

        if (State == PlayerState.Watch) return;

        switch (State)
        {
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

        SetLayerTo("1592", 7);
        SetLayerTo("1919", 7);
        SetLayerTo("Start", 7);
        SetLayerTo("UserButton", 7);

        // 상호작용 입력이 시작되고, 상호작용 가능한 오브젝트가 존재할 때
        if (callbackContext.phase == InputActionPhase.Performed && _curInteractable != null)
        {
            if (State == PlayerState.Watch)
            {
                ExitWatchState(); // Watch 상태에서 나옴
            }
            else if (_curInteractable != null)
            {
                Debug.Log("상호작용 중...");
                State = PlayerState.Watch; // Watch 상태로 진입
                _curInteractable.Interact(); // 상호작용 로직 실행
                cameraTransform.transform.position = _initialPosition;
                cameraTransform.transform.rotation = _initialRotation; 
                _interactText.gameObject.SetActive(false); // 텍스트 비활성화
                UpdateIdle();
                SetRenderersEnabled(false);
            }
        }
    }
    void SetLayerTo(string objectName, int layerNum)
    {
        // 이름으로 오브젝트를 찾음
        GameObject obj = GameObject.Find(objectName);

        if (obj != null)
        {
            // 레이어를 layerNum으로 설정
            obj.layer = layerNum;
            Debug.Log($"{objectName}의 레이어를 0으로 설정했습니다.");
        }
        else
        {
            Debug.LogWarning($"{objectName} 오브젝트를 찾을 수 없습니다.");
        }
    }

    // Watch 상태에서 나오는 함수
    void ExitWatchState()
    {
        Debug.Log("Watch 상태 종료, Idle 상태로 전환");
        State = PlayerState.Idle; // 상태를 Idle로 전환
        UpdateCameraPosition();
        SetRenderersEnabled(true);
        Managers.UI.CloseAllPopupUI();
        SetLayerTo("1592", 0);
        SetLayerTo("1919", 0);
        SetLayerTo("Start", 0);
        SetLayerTo("UserButton", 0);
    }


    // 키 감지하는 함수
    void OnKeyboard()
    {
        //// 이동 입력 처리
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
            cameraOffset.y = Mathf.Clamp(cameraOffset.y, 1f, 2.5f); // 카메라 높이 제한 (1 ~ 10)

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
            cameraTransform.LookAt(transform.position + Vector3.up * 1f);
        }
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
}
