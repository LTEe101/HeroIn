using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PlayerController;

public class HomeScene : BaseScene
{
    private Vector3 _originalScale; // 객체의 원래 Scale을 저장할 변수
    private float _highlightScaleFactor = 1.02f; // 하이라이트 시 커질 비율
    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.3f, -0.639335f); // 목표 위치
    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // 목표 회전
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
        StartCoroutine(Managers.API.GetUserCardInfo(
            Managers.Data.userInfo.userId,
            () => {
                // 카드 정보를 성공적으로 가져왔을 때 처리
                Debug.Log("카드 정보를 성공적으로 가져왔습니다.");

                // 가져온 카드 정보를 출력하거나 필요한 곳에 활용
                foreach (var card in Managers.Data.cards)
                {
                    Debug.Log($"카드 이름: {card.name}, 설명: {card.description}");
                }
            },
            (error) => {
                // 카드 정보를 가져오는 데 실패했을 때 처리
                Debug.LogError($"카드 정보 가져오기 실패: {error}");
            }
        ));
        StartCoroutine(Managers.API.GetUserTitleInfo(
            Managers.Data.userInfo.userId,
            () => {
                // 카드 정보를 성공적으로 가져왔을 때 처리
                Debug.Log("업적 정보를 성공적으로 가져왔습니다.");

                // 가져온 카드 정보를 출력하거나 필요한 곳에 활용
                foreach (var title in Managers.Data.titles)
                {
                    Debug.Log($"업적 이름: {title.userTitle}");
                }

            },
            (error) => {
                // 카드 정보를 가져오는 데 실패했을 때 처리
                Debug.LogError($"업적 정보 가져오기 실패: {error}");
            }
        ));
        Managers.Sound.Play("KarugamoBGM/KBF_Town_Indoor_G01_A", Define.Sound.Bgm, 1.5f);
    }
    public override void Clear()
    {
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
                OnPointerEnter(Managers.Input.HoveredObject); 
                break;
            case Define.MouseEvent.PointerExit:
                OnPointerExit(Managers.Input.HoveredObject);
                break;
        }
    }

    // 마우스 클릭 시 처리 메서드
    void OnMouseClicked()
    {
        if (_hitObject != null && _originalColor != default(Color))
        {
            Renderer renderer = _hitObject.GetComponent<Renderer>();
            renderer.material.color = _originalColor;
            _originalColor = default(Color);
        }

        _hitObject = Managers.Input.HoveredObject;

        // Exit는 상태에 상관없이 동작
        if (_hitObject.name == "Exit")
        {
            Managers.Sound.Play("ProEffect/Doors/door_lock_close_01", Define.Sound.Effect, 0.8f);
            Managers.Scene.LoadScene(Define.Scene.Museum);
            Debug.Log("나가기");
            return;
        }

        // 나머지 버튼은 Watch 상태일 때만 동작
        if (PlayerController.Instance.State != PlayerState.Watch)
        {
            Debug.Log("현재 Watch 상태가 아닙니다.");
            return;
        }

        // 상태가 Watch일 때 처리
        switch (_hitObject.name)
        {
            case "UserButton":
                if (_monitorStatus != MonitorStatus.Close) CloseMonitory();
                PlayButtonSound();
                StartCoroutine(MoveCameraAndShowUI<UI_User>());
                break;
            case "1592":
                OnStoryButton(MonitorStatus.Story1592, "1592", "한산도 대첩", "이순신 장군");
                break;
            case "1919":
                OnStoryButton(MonitorStatus.Story1919, "1919", "3.1운동", "유관순 열사");
                break;
            case "Start":
                Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Mouse_Huge_Generic_3", Define.Sound.Effect, 0.8f);
                LoadScene();
                break;
            default:
                Debug.Log("해당 오브젝트에 대한 팝업이 없습니다.");
                break;
        }
        
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Mouse_Huge_Generic_3", Define.Sound.Effect, 0.8f);
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
        PlayButtonSound();
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
        Debug.Log(hoveredObject.name);
        if (hoveredObject.name == "Exit")
        {
            ChangeOutlineColor(hoveredObject, Color.white);
        }
        if (PlayerController.Instance.State != PlayerState.Watch)
        {
            Debug.Log("현재 Watch 상태가 아닙니다.");
            return;
        }
        if (hoveredObject.CompareTag("InteractableButton"))
        {
            ChangeOutlineColor(hoveredObject, Color.white);
        }
    }

    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;

        if (hoveredObject.CompareTag("InteractableButton"))
        {
            ChangeOutlineColor(hoveredObject, Color.black);
        }
    }

    // 카메라 위치와 회전을 변경하고 UI를 띄우는 메서드
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();

        // 현재 카메라 위치 및 회전
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        float duration = 1.0f; // 카메라 이동에 걸리는 시간
        float elapsedTime = 0.0f;

        // 카메라를 부드럽게 목표 위치와 회전으로 이동
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // 카메라의 위치와 회전을 보간하여 부드럽게 이동
            Camera.main.transform.position = Vector3.Lerp(startPosition, _targetPosition, elapsedTime / duration);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, _targetRotation, elapsedTime / duration);

            yield return null; // 프레임마다 기다림
        }

        // 카메라 이동이 완료된 후 UI 띄우기
        Managers.UI.ShowPopupUI<T>();
    }

    public void LoadScene()
    {
        Managers.Sound.StopBGM();
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
