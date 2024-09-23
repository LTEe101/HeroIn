using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraController;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    Vector3 _destPos;

    // 카메라를 참조하는 변수
    public Transform cameraTransform;

    // 카메라의 오프셋
    [SerializeField]
    Vector3 cameraOffset = new Vector3(0, 1.5f, -4); // 적절한 높이와 거리 설정

    [SerializeField]
    float _mouseSensitivity = 2.0f; // 마우스 감도

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard;
        Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        UpdateCameraPosition();
    }

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
    }

    PlayerState _state = PlayerState.Idle;

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

        
        Debug.DrawRay(transform.position, Vector3.forward * 5.0f, Color.red); // 5.0f 길이로 앞으로 그리기
        if (Physics.Raycast(transform.position, dir.normalized, out hit, rayDistance))
        { 
            Debug.Log("오브젝트와 충돌: " + hit.collider.name);
        }
        // 플레이어 전방으로 레이캐스트를 쏨
        if (Physics.Raycast(transform.position, dir.normalized, out hit, rayDistance, wallLayer))
        {
            Debug.Log("벽과 충돌: " + hit.collider.name);
            _state = PlayerState.Idle; // 벽에 부딪히면 이동 멈춤
            return;
        }

        if (dir.magnitude < 0.0001f)
        {
            _state = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;

            // 카메라 위치 업데이트
            UpdateCameraPosition();
        }

        // 애니메이션
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);
    }


    void UpdateIdle()
    {
        // 애니메이션
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        HandleMouseRotation(); // 마우스 회전 처리
        switch (_state)
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
    }

    void OnKeyboard()
    {
        if (_state == PlayerState.Die)
            return;

        // 카메라 방향에 따라 이동할 방향을 계산
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Y축 제외하여 평면 상에서 이동하도록 설정
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir += forward; // 카메라가 보는 방향으로 이동
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir -= forward; // 카메라가 보는 반대 방향으로 이동
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
            _state = PlayerState.Moving;
        }

        // 애니메이션
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);
    }

    void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1)) // 마우스 우클릭 시
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;

            // 캐릭터와 카메라 Y축 회전
            transform.Rotate(0, mouseX, 0); // 캐릭터 회전
            cameraTransform.RotateAround(transform.position, Vector3.up, mouseX); // 카메라 회전
        }
    }

    void UpdateCameraPosition()
    {
        if (cameraTransform != null)
        {
            // 캐릭터의 회전값을 적용하여 카메라의 위치 설정
            Vector3 rotatedOffset = transform.rotation * cameraOffset;
            cameraTransform.position = transform.position + rotatedOffset;

            // 카메라가 플레이어를 바라보도록 설정 (회전값 고정)
            cameraTransform.rotation = Quaternion.Euler(15f, cameraTransform.rotation.eulerAngles.y, 0f);
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            string hitObjectName = hit.collider.gameObject.name;
            Debug.Log(hitObjectName);
            switch (hitObjectName)
            {
                case "Spaceship":
                    Managers.Scene.LoadScene(Define.Scene.Home);
                    break;
                default:
                    Debug.Log("해당 오브젝트에 대한 팝업이 없습니다.");
                    break;
            }

        }
    }
}
