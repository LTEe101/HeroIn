using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CameraController;
using static Outline;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 5.0f;

    Vector3 _destPos;

    // 카메라를 참조하는 변수
    public Transform cameraTransform;

    // 카메라의 오프셋
    [SerializeField]
    Vector3 cameraOffset = new Vector3(0, 2, -2); // 적절한 높이와 거리 설정

    [SerializeField]
    float _mouseSensitivity = 2.0f; // 마우스 감도
    void Start()
    {
        Debug.Log("시작");
        _destPos = transform.position;  // 시작 위치 설정
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
            // 후진 중이 아니면 회전
            if (!Input.GetKey(KeyCode.S)) // 후진할 때는 회전하지 않음
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 2 * Time.deltaTime);
            }
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
        HandleMovement();
        UpdateCameraPosition(); 
    }

    void HandleMovement()
    {
        if (_state == PlayerState.Die)
            return;

        Vector3 moveDir = Vector3.zero;

        // 카메라 방향에 따라 이동할 방향을 계산
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

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
            _state = PlayerState.Moving;
        }
    }
    void UpdateCameraPosition()
    {
        if (cameraTransform != null)
        {
            // 캐릭터의 정면 기준으로 카메라를 회전시켜서 캐릭터의 뒤에 위치시킴
            Vector3 targetPosition = transform.position + (transform.rotation * cameraOffset); // 캐릭터의 회전값을 반영한 오프셋 위치
            cameraTransform.position = targetPosition;

            // 카메라가 캐릭터를 항상 바라보도록 설정
            cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }
}
