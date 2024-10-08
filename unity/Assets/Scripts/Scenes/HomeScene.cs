using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PlayerController;

public class HomeScene : BaseScene
{
    private Vector3 _originalScale; // ��ü�� ���� Scale�� ������ ����
    private float _highlightScaleFactor = 1.02f; // ���̶���Ʈ �� Ŀ�� ����
    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.3f, -0.639335f); // ��ǥ ��ġ
    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // ��ǥ ȸ��
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

        // main_tv ������Ʈ�� ã�ų� �ν����Ϳ��� ����
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
                // ī�� ������ ���������� �������� �� ó��
                Debug.Log("ī�� ������ ���������� �����Խ��ϴ�.");

                // ������ ī�� ������ ����ϰų� �ʿ��� ���� Ȱ��
                foreach (var card in Managers.Data.cards)
                {
                    Debug.Log($"ī�� �̸�: {card.name}, ����: {card.description}");
                }
            },
            (error) => {
                // ī�� ������ �������� �� �������� �� ó��
                Debug.LogError($"ī�� ���� �������� ����: {error}");
            }
        ));
        StartCoroutine(Managers.API.GetUserTitleInfo(
            Managers.Data.userInfo.userId,
            () => {
                // ī�� ������ ���������� �������� �� ó��
                Debug.Log("���� ������ ���������� �����Խ��ϴ�.");

                // ������ ī�� ������ ����ϰų� �ʿ��� ���� Ȱ��
                foreach (var title in Managers.Data.titles)
                {
                    Debug.Log($"���� �̸�: {title.userTitle}");
                }

            },
            (error) => {
                // ī�� ������ �������� �� �������� �� ó��
                Debug.LogError($"���� ���� �������� ����: {error}");
            }
        ));
        Managers.Sound.Play("KarugamoBGM/KBF_Town_Indoor_G01_A", Define.Sound.Bgm, 1.5f);
    }
    public override void Clear()
    {
    }

    // ���콺 �Է� ó�� �޼���
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

    // ���콺 Ŭ�� �� ó�� �޼���
    void OnMouseClicked()
    {
        if (_hitObject != null && _originalColor != default(Color))
        {
            Renderer renderer = _hitObject.GetComponent<Renderer>();
            renderer.material.color = _originalColor;
            _originalColor = default(Color);
        }

        _hitObject = Managers.Input.HoveredObject;

        // Exit�� ���¿� ������� ����
        if (_hitObject.name == "Exit")
        {
            Managers.Sound.Play("ProEffect/Doors/door_lock_close_01", Define.Sound.Effect, 0.8f);
            Managers.Scene.LoadScene(Define.Scene.Museum);
            Debug.Log("������");
            return;
        }

        // ������ ��ư�� Watch ������ ���� ����
        if (PlayerController.Instance.State != PlayerState.Watch)
        {
            Debug.Log("���� Watch ���°� �ƴմϴ�.");
            return;
        }

        // ���°� Watch�� �� ó��
        switch (_hitObject.name)
        {
            case "UserButton":
                if (_monitorStatus != MonitorStatus.Close) CloseMonitory();
                PlayButtonSound();
                StartCoroutine(MoveCameraAndShowUI<UI_User>());
                break;
            case "1592":
                OnStoryButton(MonitorStatus.Story1592, "1592", "�ѻ굵 ��ø", "�̼��� �屺");
                break;
            case "1919":
                OnStoryButton(MonitorStatus.Story1919, "1919", "3.1�", "������ ����");
                break;
            case "Start":
                Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Mouse_Huge_Generic_3", Define.Sound.Effect, 0.8f);
                LoadScene();
                break;
            default:
                Debug.Log("�ش� ������Ʈ�� ���� �˾��� �����ϴ�.");
                break;
        }
        
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Mouse_Huge_Generic_3", Define.Sound.Effect, 0.8f);
    }

    // ��ư �÷��� ��� �ϴ� �޼���
    void ChangeColor()
    {
        Renderer renderer = _hitObject.GetComponent<Renderer>();
        _originalColor = renderer.material.color;
        renderer.material.color = _originalColor * 1.3f;
    }

    // ���丮 ��ư �޼���
    void OnStoryButton(MonitorStatus status, string storyYear, string storyName, string historicalFigure)
    {
        PlayButtonSound();
        if (_monitorStatus == status) // ȭ�鿡 ��µǾ� �ִ� ȭ�� ��ư ���� ���
        {
            CloseMonitory();
        }
        else // �ƴ� ���
        {
            ShowMonitor(status, storyYear, storyName, historicalFigure);
            ChangeColor();
        }
    }

    // ����� ȭ�� On
    void ShowMonitor(MonitorStatus status, string storyYear, string storyName, string historicalFigure)
    {
        _storyYear.text = storyYear;
        _storyName.text = storyName;
        _historicalFigure.text = historicalFigure;
        _monitorStatus = status;
    }

    // ����� ȭ�� Off
    void CloseMonitory()
    {
        _storyYear.text = "";
        _storyName.text = "";
        _historicalFigure.text = "";
        _monitorStatus = MonitorStatus.Close;
    }

    void ChangeOutlineColor(GameObject go, Color newColor)
    {
        // ������Ʈ���� Outline ������Ʈ�� ������
        Outline outline = go.GetComponent<Outline>();

        // Outline ������Ʈ�� ������ ���� ����
        if (outline != null)
        {
            // �ƿ����� �÷� ����
            outline.OutlineColor = newColor;
        }
        else
        {
            Debug.LogWarning("Outline ������Ʈ�� ã�� �� �����ϴ�.");
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
            Debug.Log("���� Watch ���°� �ƴմϴ�.");
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

    // ī�޶� ��ġ�� ȸ���� �����ϰ� UI�� ���� �޼���
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();

        // ���� ī�޶� ��ġ �� ȸ��
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        float duration = 1.0f; // ī�޶� �̵��� �ɸ��� �ð�
        float elapsedTime = 0.0f;

        // ī�޶� �ε巴�� ��ǥ ��ġ�� ȸ������ �̵�
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // ī�޶��� ��ġ�� ȸ���� �����Ͽ� �ε巴�� �̵�
            Camera.main.transform.position = Vector3.Lerp(startPosition, _targetPosition, elapsedTime / duration);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, _targetRotation, elapsedTime / duration);

            yield return null; // �����Ӹ��� ��ٸ�
        }

        // ī�޶� �̵��� �Ϸ�� �� UI ����
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
                Debug.Log("�̵�");
                break;
            default:
                break;
        }
    }
}
