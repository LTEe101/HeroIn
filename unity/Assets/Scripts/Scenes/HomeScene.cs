using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScene : BaseScene
{
    [SerializeField]
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    [SerializeField]
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전

    [SerializeField]
    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // 목표 위치

    [SerializeField]
    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // 목표 회전

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Home;

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        // Singleton 패턴으로 접근
        if (CameraController.Instance != null)
        {
            // 초기 위치 및 회전 설정
            CameraController.Instance.transform.position = _initialPosition;
            CameraController.Instance.transform.rotation = _initialRotation;
        }
        else
        {
            Debug.LogError("CameraController 인스턴스를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // CameraController의 상태가 Move이거나 UI 스택이 존재하면 회전하지 않음
        if (CameraController.Instance.Status == CameraController.CameraStatus.Move || Managers.UI.GetStackCount() > 0) return;

        HandleMouseRotation(); // 마우스 회전 처리
    }

    void HandleMouseRotation()
    {
        if (CameraController.Instance == null) return;

        // 마우스 입력을 받아 카메라 회전
        if (Input.GetMouseButton(1)) // 마우스 오른쪽 버튼을 누르고 있을 때만 회전
        {
            float mouseX = Input.GetAxis("Mouse X");
            CameraController.Instance.RotateCamera(mouseX);
        }
    }

    public override void Clear()
    {
        // 필요 시 구현
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (evt != Define.MouseEvent.Click) return;
        if (CameraController.Instance == null) return;

        Ray ray = CameraController.Instance.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // CameraController의 상태가 Move인 경우 클릭 무시
        if (CameraController.Instance.Status == CameraController.CameraStatus.Move)
        {
            Debug.Log("카메라 이동이 끝나고 클릭하세요");
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
                    // 이동
                    LoadScene(topPopup);
                    break;
                case "Exit":
                    Managers.Scene.LoadScene(Define.Scene.Museum);
                    Debug.Log("나가기");
                    break;
                default:
                    Debug.Log("해당 오브젝트에 대한 팝업이 없습니다.");
                    break;
            }
        }
    }

    // 카메라 위치와 회전을 변경하고 UI를 띄우는 메서드
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();
        // 카메라 이동
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // 이동이 완료된 후 UI 띄우기
        ShowPopupUI<T>();
    }

    // 카메라 초기 위치와 회전으로 이동하고 UI를 띄우는 메서드
    IEnumerator MoveToInitialPositionAndRotationAndShowUI<T>() where T : UI_Popup
    {
        // 카메라 이동
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_initialPosition, _initialRotation));

        // 이동이 완료된 후 UI 띄우기
        ShowPopupUI<T>();
    }

    public void ShowPopupUI<T>() where T : UI_Popup
    {
        UI_Popup topPopup = Managers.UI.GetTopPopupUI();
        if (topPopup != null && topPopup.GetType() == typeof(T))
        {
            Managers.UI.CloseAllPopupUI();
            return; // 이미 같은 팝업이 열려 있으면 아무 작업도 하지 않음
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
            Debug.Log("이동");
        }
    }

    // 카메라를 원래 위치과 회전으로 돌아가게 하는 메서드
    public void MoveToInitialPositionAndRotation()
    {
        StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_initialPosition, _initialRotation));
    }
}
