using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeScene : BaseScene
{
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // �ʱ� ȸ��

    private Vector3 _targetPosition = new Vector3(-10.18889f, 2.267206f, -0.639335f); // ��ǥ ��ġ

    private Quaternion _targetRotation = Quaternion.Euler(-1.082f, 0.104f, -0.001f); // ��ǥ ȸ��

    private Vector3 _originalScale; // ��ü�� ���� Scale�� ������ ����

    private float _highlightScaleFactor = 1.02f; // ���̶���Ʈ �� Ŀ�� ����

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

    }


    public override void Clear()
    {
        // �ʿ� �� ����
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
                OnPointerEnter(Managers.Input.HoveredObject); // ���� ȣ���� ��ü ��� �ϱ�
                break;
            case Define.MouseEvent.PointerExit:
                OnPointerExit(Managers.Input.HoveredObject); // ���̶���Ʈ ����
                break;
        }
    }
    
    // ���콺 Ŭ�� �� ó�� �޼���
    void OnMouseClicked()
    {
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
                    OnStoryButton(MonitorStatus.Story1592, "1592", "�ѻ굵 ��ø", "�̼��� �屺");
                    break;
                case "1919":
                    OnStoryButton(MonitorStatus.Story1919, "1919", "3.1�", "������ ����");
                    break;
                case "Start":
                    LoadScene();
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

    // ���̶���Ʈ�� ���� ���·� �����ϴ� �Լ�
    void ResetHighlight(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // ���� Scale�� ����
            hoveredObject.transform.localScale = _originalScale;
            hoveredObject = null; // ���̶���Ʈ ��ü �ʱ�ȭ

        }
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

    // ���콺 ȣ�� �� ��ü�� Ű��� UI_Enter ����ϴ� �Լ�
    void HighlightObject(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // ���� Scale ����
            _originalScale = hoveredObject.transform.localScale;

            // ���̶���Ʈ Scale ����
            hoveredObject.transform.localScale = _originalScale * _highlightScaleFactor; // Scale ����
        }
    }

    // ī�޶� ��ġ�� ȸ���� �����ϰ� UI�� ���� �޼���
    IEnumerator MoveCameraAndShowUI<T>() where T : UI_Popup
    {
        Managers.UI.CloseAllPopupUI();
        // ī�޶� �̵�
        yield return StartCoroutine(CameraController.Instance.MoveToPositionAndRotation(_targetPosition, _targetRotation));

        // �̵��� �Ϸ�� �� UI ����
        Managers.UI.ShowPopupUI<T>();
    }

    public void LoadScene()
    {
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
