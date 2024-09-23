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
