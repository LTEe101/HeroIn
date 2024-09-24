using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScene : BaseScene
{
    [SerializeField]
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    [SerializeField]
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // �ʱ� ȸ��

    [SerializeField]
    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // ��ǥ ��ġ

    [SerializeField]
    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // ��ǥ ȸ��

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Home;

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        // Singleton �������� ����
        if (CameraController.Instance != null)
        {
            // �ʱ� ��ġ �� ȸ�� ����
            CameraController.Instance.transform.position = _initialPosition;
            CameraController.Instance.transform.rotation = _initialRotation;
        }
        else
        {
            Debug.LogError("CameraController �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        // CameraController�� ���°� Move�̰ų� UI ������ �����ϸ� ȸ������ ����
        if (CameraController.Instance.Status == CameraController.CameraStatus.Move || Managers.UI.GetStackCount() > 0) return;

        HandleMouseRotation(); // ���콺 ȸ�� ó��
    }

    void HandleMouseRotation()
    {
        if (CameraController.Instance == null) return;

        // ���콺 �Է��� �޾� ī�޶� ȸ��
        if (Input.GetMouseButton(1)) // ���콺 ������ ��ư�� ������ ���� ���� ȸ��
        {
            float mouseX = Input.GetAxis("Mouse X");
            CameraController.Instance.RotateCamera(mouseX);
        }
    }

    public override void Clear()
    {
        // �ʿ� �� ����
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (evt != Define.MouseEvent.Click) return;
        if (CameraController.Instance == null) return;

        Ray ray = CameraController.Instance.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // CameraController�� ���°� Move�� ��� Ŭ�� ����
        if (CameraController.Instance.Status == CameraController.CameraStatus.Move)
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
                    Managers.Scene.LoadScene(Define.Scene.Museum);
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
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // �̵��� �Ϸ�� �� UI ����
        ShowPopupUI<T>();
    }

    // ī�޶� �ʱ� ��ġ�� ȸ������ �̵��ϰ� UI�� ���� �޼���
    IEnumerator MoveToInitialPositionAndRotationAndShowUI<T>() where T : UI_Popup
    {
        // ī�޶� �̵�
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_initialPosition, _initialRotation));

        // �̵��� �Ϸ�� �� UI ����
        ShowPopupUI<T>();
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
            //Managers.Scene.LoadingLoadScene(Define.Scene.Game);
            // SceneManager.LoadScene();
            Debug.Log("�̵�");
        }
    }

    // ī�޶� ���� ��ġ�� ȸ������ ���ư��� �ϴ� �޼���
    public void MoveToInitialPositionAndRotation()
    {
        StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_initialPosition, _initialRotation));
    }
}
