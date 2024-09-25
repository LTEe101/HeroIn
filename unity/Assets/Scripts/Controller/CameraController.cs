using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // ī�޶� ���¸� ��Ÿ���� ������
    public enum CameraStatus
    {
        Stop,
        Move,
    }

    // ���� ī�޶� ����
    private CameraStatus _status = CameraStatus.Stop;

    // ���� ���¸� �ܺο��� ���� �� �ֵ��� ������Ƽ �߰�
    public CameraStatus Status
    {
        get { return _status; }
    }

    [SerializeField] Vector3 startPosition; // 카메라 시작 위치
    [SerializeField] Vector3 closeUpPosition; // 클로즈업할 위치 (카메라가 이동할 위치)
    [SerializeField] Quaternion startRotation; // 카메라 시작 회전
    [SerializeField] Quaternion closeUpRotation; // 클로즈업할 회전
    [SerializeField] float movingSpeed = 1.0f; // 카메라 이동 속도
    private bool isMoving = false; // 카메라가 움직이는지 여부
    public System.Action onCloseUpComplete;

    [SerializeField]
    private float _moveSpeed = 5.0f; // ī�޶� �̵� �ӵ�

    [SerializeField]
    private float _rotationSpeed = 2.0f; // ī�޶� ȸ�� �ӵ�

    [SerializeField]
    private float _mouseSensitivity = 2.0f; // ���콺 ����

    // Singleton �ν��Ͻ�
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        // Singleton ���� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
            return;
        }
    }

    void Start()
    {
        _status = CameraStatus.Stop;

        Camera[] allCameras = Camera.allCameras;
        foreach (Camera cam in allCameras)
        {
            if (cam != GetComponent<Camera>())
            {
                cam.enabled = false;
            }
        }

        // 카메라의 시작 위치 설정
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
    void LateUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, closeUpPosition, Time.deltaTime * movingSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, closeUpRotation, Time.deltaTime * movingSpeed);

            // 클로즈업 위치에 도달하면 움직임을 멈춤
            if (Vector3.Distance(transform.position, closeUpPosition) < 0.1f)
            {
                isMoving = false;
                onCloseUpComplete?.Invoke();
            }
        }
    }
    public void StartCloseUp(Vector3 start, Vector3 end, Quaternion startRot, Quaternion endRot, float speed)
    {
        startPosition = start;
        closeUpPosition = end;
        startRotation = startRot;
        closeUpRotation = endRot;
        movingSpeed = speed;
        isMoving = true;
    }

    // ī�޶� ��ǥ ��ġ�� ȸ������ �̵���Ű�� �ڷ�ƾ
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

        // ���� ��ġ �� ȸ�� ����
        transform.position = targetPos;
        transform.rotation = targetRot;
        _status = CameraStatus.Stop;
    }

    // ���콺 �Է¿� ���� ī�޶� ȸ�� ó��
    public void RotateCamera(float mouseX)
    {
        if (_status == CameraStatus.Move)
            return;

        Vector3 rotation = transform.localEulerAngles;
        rotation.y -= mouseX * _mouseSensitivity;

        // Y�� ȸ�� ���� (��: 310�� ~ 2��)
        if (rotation.y >= 310 || rotation.y <= 2)
        {
            transform.localEulerAngles = new Vector3(6, rotation.y, 0);
        }
    }

    // ī�޶� �̵��� �����ϴ� �޼���
    public void StartMoveToPositionAndRotation(Vector3 targetPos, Quaternion targetRot)
    {
        StartCoroutine(MoveToPositionAndRotation(targetPos, targetRot));
    }
}
