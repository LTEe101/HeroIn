using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라 상태를 나타내는 열거형
    public enum CameraStatus
    {
        Stop,
        Move,
    }

    // 현재 카메라 상태
    private CameraStatus _status = CameraStatus.Stop;

    // 현재 상태를 외부에서 읽을 수 있도록 프로퍼티 추가
    public CameraStatus Status
    {
        get { return _status; }
    }

    [SerializeField]
    private float _moveSpeed = 5.0f; // 카메라 이동 속도

    [SerializeField]
    private float _rotationSpeed = 2.0f; // 카메라 회전 속도

    [SerializeField]
    private float _mouseSensitivity = 2.0f; // 마우스 감도

    // Singleton 인스턴스
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
            return;
        }
    }

    void Start()
    {
        _status = CameraStatus.Stop;
    }

    // 카메라를 목표 위치와 회전으로 이동시키는 코루틴
    public IEnumerator MoveToPositionAndRotation(Vector3 targetPos, Quaternion targetRot)
    {
        _status = CameraStatus.Move;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f ||
               Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // 최종 위치 및 회전 조정
        transform.position = targetPos;
        transform.rotation = targetRot;
        _status = CameraStatus.Stop;
    }

    // 마우스 입력에 따라 카메라 회전 처리
    public void RotateCamera(float mouseX)
    {
        if (_status == CameraStatus.Move)
            return;

        Vector3 rotation = transform.localEulerAngles;
        rotation.y -= mouseX * _mouseSensitivity;

        // Y축 회전 제한 (예: 310도 ~ 2도)
        if (rotation.y >= 310 || rotation.y <= 2)
        {
            transform.localEulerAngles = new Vector3(6, rotation.y, 0);
        }
    }

    // 카메라 이동을 시작하는 메서드
    public void StartMoveToPositionAndRotation(Vector3 targetPos, Quaternion targetRot)
    {
        StartCoroutine(MoveToPositionAndRotation(targetPos, targetRot));
    }
}
