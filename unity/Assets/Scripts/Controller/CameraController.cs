using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    [SerializeField]
    Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // �ʱ� ȸ��

    [SerializeField]
    Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // ��ǥ ��ġ

    [SerializeField]
    Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // ��ǥ ȸ��

    [SerializeField]
    float _moveSpeed = 5.0f; // ī�޶� �̵� �ӵ�

    [SerializeField]
    float _rotationSpeed = 2.0f; // ī�޶� ȸ�� �ӵ�

    [SerializeField]
    float _mouseSensitivity = 2.0f; // ���콺 ����
    public enum Cameras
    {
        Stop,
        Move,
    }
    Cameras _status;

    void Start()
    {
        // �ʱ� ��ġ ����
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        _status = Cameras.Stop;
    }
    void Update()
    {
        if (_status == Cameras.Move || Managers.UI.GetStackCount() > 0) return;
        HandleMouseRotation(); // ���콺 ȸ�� ó��
    }

    void HandleMouseRotation()
    {
        // ���콺 �Է��� �޾� ī�޶� ȸ��
        if (Input.GetMouseButton(1) && _status == Cameras.Stop) // ���콺 ������ ��ư�� ������ ���� ���� ȸ��
        {
            _status = Cameras.Move;
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;

            // ���� ���� ȸ�� ���� ������
            Vector3 rotation = transform.localEulerAngles;

            // Y�� ���� ������Ʈ�Ͽ� 90���� ȸ���ϵ��� ����
            rotation.y -= mouseX;
            if (rotation.y >= 310 || rotation.y <= 2)
            {
                // ȸ�� �� ���� (Y�ุ ȸ��)
                transform.localEulerAngles = new Vector3(6, rotation.y, 0);
            }
            _status = Cameras.Stop;
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _initialPosition = delta;
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (_status == Cameras.Move)
        {
            Debug.Log("ī�޶� �̵��� ������ Ŭ���ϼ���");
            return;
        }

        if (Physics.Raycast(ray, out hit))
        {
            string hitObjectName = hit.collider.gameObject.name;
            Debug.Log(hitObjectName);
            switch (hitObjectName)
            {
                case "UserButton":
                    StartCoroutine(MoveCameraAndShowUI<UI_User>());
                    break;
                case "1592":
                    StartCoroutine(MoveToInitialPositionAndRotationAndShowUI<UI_Story_Info_1592>());
                    break;
                case "1919":
                    StartCoroutine(MoveToInitialPositionAndRotationAndShowUI<UI_Story_Info_1919>());
                    break;
                case "Start":
                    UI_Popup topPopup = Managers.UI.GetTopPopupUI();
                    if (topPopup == null) break;
                    // �̵�
                    LoadScene(topPopup);
                    break;
                case "Exit":
                    Debug.Log("������");
                    break;
                default:
                    Debug.Log("�ش� ������Ʈ�� ���� �˾��� �����ϴ�.");
                    break;
            }
           
        }
    }

    // ī�޶� ��ġ�� ȸ���� �����ϰ� UI�� ���� �޼���
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();
        // ī�޶� �̵�
        yield return StartCoroutine(MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // �̵��� �Ϸ�� �� UI ����
        ShowPopupUI<T>();
    }

    // ī�޶� �ʱ� ��ġ�� ȸ������ �̵��ϰ� UI�� ���� �޼���
    IEnumerator MoveToInitialPositionAndRotationAndShowUI<T>() where T : UI_Popup
    {
        // ī�޶� �̵�
        yield return StartCoroutine(MoveToPositionAndRotation(_initialPosition, _initialRotation));

        // �̵��� �Ϸ�� �� UI ����
        ShowPopupUI<T>();
    }

    // ī�޶� ��ǥ ��ġ�� ȸ������ �̵���Ű�� �ڷ�ƾ
    IEnumerator MoveToPositionAndRotation(Vector3 targetPos, Quaternion targetRot)
    {
        _status = Cameras.Move;
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
        _status = Cameras.Stop;
    }

    // ī�޶� ���� ��ġ�� ȸ������ ���ư��� �ϴ� �޼���
    public void MoveToInitialPositionAndRotation()
    {
        StartCoroutine(MoveToPositionAndRotation(_initialPosition, _initialRotation));
    }

    public void ShowPopupUI<T>() where T : UI_Popup
    {
        UI_Popup topPopup = Managers.UI.GetTopPopupUI();
        if (topPopup != null && topPopup.GetType() == typeof(T))
        {
            Managers.UI.CloseAllPopupUI();
            return; // �̹� ���� �˾��� ���� ������ �ƹ� �۾��� ���� ����
        }
        else if (Managers.UI.GetStackCount() > 0)
        {
            Managers.UI.CloseAllPopupUI();
        }
        Managers.UI.ShowPopupUI<T>();
    }
    public void LoadScene(UI_Popup topPopup)
    {
        if (topPopup.GetType() == typeof(UI_Story_Info_1592))
        {
            //Managers.Scene.LodingLoadScene(Define.Scene.Game);
            // SceneManager.LoadScene();
            Debug.Log("�̵�");
        }
    }
}
