using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static CameraController;
using static Outline;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 5.0f;
    public static PlayerController Instance { get; private set; }
    [SerializeField] private TMP_Text _interactText; // 상호작용 가능할 때 나타나는 텍스트 UI
    [SerializeField] private float _checkRate = 0.05f; // 광선을 쏘는 주기 (0.05초마다)
    [SerializeField] private float _maxDistance = 3.0f; // 광선이 감지할 수 있는 최대 거리
    [SerializeField] private LayerMask _layerMask; // 상호작용할 수 있는 오브젝트를 감지할 레이어
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f); // 초기 위치
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전

    Vector3 _destPos;
    private float _lastCheckTime; // 마지막으로 광선을 쏜 시간을 기록

    private GameObject _curGameobject; // 현재 감지된 오브젝트
    private IInteractable _curInteractable; // 현재 상호작용 가능한 인터페이스를 구현한 오브젝트

    // 카메라를 참조하는 변수
    private Camera _camera;

    // 카메라의 오프셋
    [SerializeField]
    Vector3 cameraOffset = new Vector3(0, 2, -2); // 적절한 높이와 거리 설정

    [SerializeField]
    float _mouseSensitivity = 2.0f; // 마우스 감도
    private PlayerInput playerInput; // PlayerInput 변수 선언

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("PlayerController 인스턴스는 하나만 존재해야 합니다.");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _destPos = transform.position;  // 시작 위치 설정
        _camera = Camera.main;
        _layerMask = LayerMask.GetMask("Interactable"); // "Interactable" 레이어에 속하는 오브젝트만 감지하도록 설정
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Interaction"].performed += OnInteraction;
        UpdateCameraPosition();
    }
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Watch,
    }
    public PlayerState State { get; private set; } = PlayerState.Idle;

    void UpdateDie()
    {
        // 아무것도 못함
    }

    void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;

        // 레이캐스트를 사용하여 플레이어 전방에 벽이 있는지 확인
        RaycastHit hit;
        float rayDistance = 1.0f; // 레이캐스트의 거리
        LayerMask wallLayer = LayerMask.GetMask("Wall"); // Wall 레이어 설정
       
        // 플레이어 전방으로 레이캐스트를 쏨
        if (Physics.Raycast(transform.position, dir.normalized, out hit, rayDistance, wallLayer | _layerMask))
        {
            Debug.Log("벽과 충돌: " + hit.collider.name);
            State = PlayerState.Idle; // 벽에 부딪히면 이동 멈춤
            return;
        }
        RayInteractable();
        if (dir.magnitude < 0.0001f)
        {
            State = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            // 후진 중이 아니면 회전
            if (!Input.GetKey(KeyCode.S)) // 후진할 때는 회전하지 않음
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 2 * Time.deltaTime);
            }
        }

        // 애니메이션
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);
        UpdateCameraPosition();
    }

    void UpdateIdle()
    {
        // 애니메이션
        Animator anim = GetComponent<Animator>();
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
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
        }

        HandleMovement();
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
                _camera.transform.position = _initialPosition;
                _camera.transform.rotation = _initialRotation; 
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

    void HandleMovement()
    {
        if (State == PlayerState.Die)
            return;

        Vector3 moveDir = Vector3.zero;

        // 카메라 방향에 따라 이동할 방향을 계산
        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        // Y축 제외하여 평면 상에서 이동하도록 설정
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // WASD 입력 처리
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += forward; // 카메라가 보는 방향으로 전진
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir -= forward; // 카메라가 보는 방향의 반대 방향으로 후진
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir -= right; // 카메라 왼쪽으로 이동
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += right; // 카메라 오른쪽으로 이동
        }

        if (moveDir != Vector3.zero)
        {
            _destPos = transform.position + moveDir.normalized * _speed * Time.deltaTime;
            State = PlayerState.Moving;
        }
    }
    void UpdateCameraPosition()
    {
        if (_camera != null)
        {
            // 캐릭터의 정면 기준으로 카메라를 회전시켜서 캐릭터의 뒤에 위치시킴
            Vector3 targetPosition = transform.position + (transform.rotation * cameraOffset); // 캐릭터의 회전값을 반영한 오프셋 위치
            _camera.transform.position = targetPosition;

            // 카메라가 캐릭터를 항상 바라보도록 설정
            _camera.transform.LookAt(transform.position + Vector3.up * 1.5f);
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
