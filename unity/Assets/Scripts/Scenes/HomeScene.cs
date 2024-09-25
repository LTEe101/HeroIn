using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeScene : BaseScene
{
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전

    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // 목표 위치

    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // 목표 회전

    private Vector3 _originalScale; // 객체의 원래 Scale을 저장할 변수

    private float _highlightScaleFactor = 1.02f; // 하이라이트 시 커질 비율

    private GameObject _mainTV;
    private TextMeshPro _storyYear;
    private TextMeshPro _storyName;
    private TextMeshPro _historicalFigure;
    private Color _originalColor;
    private GameObject _hitObject;
    enum MonitorStatus
    {
        Close,
        Story1592,
        Story1919,
    }

    MonitorStatus _monitorStatus = MonitorStatus.Close;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Home;

        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;

        // main_tv 오브젝트를 찾거나 인스펙터에서 연결
        if (_mainTV == null)
        {
            _mainTV = GameObject.Find("main_tv");
            GameObject storyYearObject = Util.FindChild(_mainTV, "StoryYear");
            GameObject storyNameObject = Util.FindChild(_mainTV, "StoryName");
            GameObject historicalFigureObject = Util.FindChild(_mainTV, "HistoricalFigure");
            _storyYear = storyYearObject.GetComponent<TextMeshPro>();
            _storyName = storyNameObject.GetComponent<TextMeshPro>();
            _historicalFigure = historicalFigureObject.GetComponent<TextMeshPro>();
        }

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

    }


    public override void Clear()
    {
        // 필요 시 구현
    }

    // 마우스 입력 처리 메서드
    void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Click:
                OnMouseClicked();
                break;
            case Define.MouseEvent.PointerEnter:
                OnPointerEnter(Managers.Input.HoveredObject); // 현재 호버된 객체 밝게 하기
                break;
            case Define.MouseEvent.PointerExit:
                OnPointerExit(Managers.Input.HoveredObject); // 하이라이트 리셋
                break;
        }
    }
    
    // 마우스 클릭 시 처리 메서드
    void OnMouseClicked()
    {
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
            if (_hitObject != null && _originalColor != default(Color))
            {
                Renderer renderer = _hitObject.GetComponent<Renderer>();
                renderer.material.color = _originalColor;
                _originalColor = default(Color); 
            }

            _hitObject = hit.collider.gameObject;

            switch (_hitObject.name)
            {
                case "UserButton":
                    if (_monitorStatus != MonitorStatus.Close) CloseMonitory();
                    StartCoroutine(MoveCameraAndShowUI<UI_User>());
                    break;
                case "1592":
                    OnStoryButton(MonitorStatus.Story1592, "1592", "한산도 대첩", "이순신 장군");
                    break;
                case "1919":
                    OnStoryButton(MonitorStatus.Story1919, "1919", "3.1운동", "유관순 열사");
                    break;
                case "Start":
                    LoadScene();
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

    // 버튼 컬러를 밝게 하는 메서드
    void ChangeColor()
    {
        Renderer renderer = _hitObject.GetComponent<Renderer>();
        _originalColor = renderer.material.color;
        renderer.material.color = _originalColor * 1.3f;
    }

    // 스토리 버튼 메서드
    void OnStoryButton(MonitorStatus status, string storyYear, string storyName, string historicalFigure)
    {
        if (_monitorStatus == status) // 화면에 출력되어 있는 화면 버튼 누른 경우
        {
            CloseMonitory();
        }
        else // 아닌 경우
        {
            ShowMonitor(status, storyYear, storyName, historicalFigure);
            ChangeColor();
        }
    }

    // 모니테 화면 On
    void ShowMonitor(MonitorStatus status, string storyYear, string storyName, string historicalFigure)
    {
        _storyYear.text = storyYear;
        _storyName.text = storyName;
        _historicalFigure.text = historicalFigure;
        _monitorStatus = status;
    }

    // 모니터 화면 Off
    void CloseMonitory()
    {
        _storyYear.text = "";
        _storyName.text = "";
        _historicalFigure.text = "";
        _monitorStatus = MonitorStatus.Close;
    }

    // 하이라이트를 원래 상태로 복구하는 함수
    void ResetHighlight(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // 원래 Scale로 복구
            hoveredObject.transform.localScale = _originalScale;
            hoveredObject = null; // 하이라이트 객체 초기화

        }
    }

    void ChangeOutlineColor(GameObject go, Color newColor)
    {
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

    void OnPointerEnter(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        switch (hoveredObject.name)
        {
            case "UserButton":
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "1592":
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "1919":
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "Start":
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "Exit":
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            default:
                break;

        }
    }
    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        switch (hoveredObject.name)
        {
            case "UserButton":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "1592":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "1919":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "Start":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "Exit":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            default:
                break;
        }
    }

    // 마우스 호버 시 객체를 키우고 UI_Enter 출력하는 함수
    void HighlightObject(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // 원래 Scale 저장
            _originalScale = hoveredObject.transform.localScale;

            // 하이라이트 Scale 적용
            hoveredObject.transform.localScale = _originalScale * _highlightScaleFactor; // Scale 증가
        }
    }

    // 카메라 위치와 회전을 변경하고 UI를 띄우는 메서드
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();
        // 카메라 이동
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // 이동이 완료된 후 UI 띄우기
        Managers.UI.ShowPopupUI<T>();
    }

    public void LoadScene()
    {
        switch (_monitorStatus)
        {
            case MonitorStatus.Story1592:
                ChangeColor();
                Managers.Scene.LodingLoadScene(Define.Scene.BeforeStory);
                Debug.Log("이동");
                break;
            default:
                break;
        }
    }
}
