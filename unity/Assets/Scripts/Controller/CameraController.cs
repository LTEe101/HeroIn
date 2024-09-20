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
    Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전

    [SerializeField]
    Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // 목표 위치

    [SerializeField]
    Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // 목표 회전

    [SerializeField]
    float _moveSpeed = 5.0f; // 카메라 이동 속도

    [SerializeField]
    float _rotationSpeed = 2.0f; // 카메라 회전 속도

    [SerializeField]
    float _mouseSensitivity = 2.0f; // 마우스 감도
    public enum Cameras
    {
        Stop,
        Move,
    }
    Cameras _status;

    void Start()
    {
        // 초기 위치 설정
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        _status = Cameras.Stop;
    }
    void Update()
    {
        if (_status == Cameras.Move || Managers.UI.GetStackCount() > 0) return;
        HandleMouseRotation(); // 마우스 회전 처리
    }

    void HandleMouseRotation()
    {
        // 마우스 입력을 받아 카메라 회전
        if (Input.GetMouseButton(1) && _status == Cameras.Stop) // 마우스 오른쪽 버튼을 누르고 있을 때만 회전
        {
            _status = Cameras.Move;
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;

            // 현재 로컬 회전 값을 가져옴
            Vector3 rotation = transform.localEulerAngles;

            // Y축 값만 업데이트하여 90도만 회전하도록 제한
            rotation.y -= mouseX;
            if (rotation.y >= 310 || rotation.y <= 2)
            {
                // 회전 값 적용 (Y축만 회전)
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
        yield return StartCoroutine(MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // 이동이 완료된 후 UI 띄우기
        ShowPopupUI<T>();
    }

    // 카메라 초기 위치와 회전으로 이동하고 UI를 띄우는 메서드
    IEnumerator MoveToInitialPositionAndRotationAndShowUI<T>() where T : UI_Popup
    {
        // 카메라 이동
        yield return StartCoroutine(MoveToPositionAndRotation(_initialPosition, _initialRotation));

        // 이동이 완료된 후 UI 띄우기
        ShowPopupUI<T>();
    }

    // 카메라를 목표 위치와 회전으로 이동시키는 코루틴
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

        // 최종 위치 및 회전 조정
        transform.position = targetPos;
        transform.rotation = targetRot;
        _status = Cameras.Stop;
    }

    // 카메라를 원래 위치와 회전으로 돌아가게 하는 메서드
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
            //Managers.Scene.LodingLoadScene(Define.Scene.Game);
            // SceneManager.LoadScene();
            Debug.Log("이동");
        }
    }
}
