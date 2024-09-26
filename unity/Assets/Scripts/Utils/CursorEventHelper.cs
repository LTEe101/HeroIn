using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class CursorEventHelper
{
    private static Texture2D _defaultCursor;
    private static Texture2D _hoverCursor;
    private static Vector2 _cursorHotspot = new Vector2(0, 0);  // Ŀ�� �߽���
    private static int _cursorWidth = 128;    // Ŀ�� �ʺ�
    private static int _cursorHeight = 128;   // Ŀ�� ����

    private static CursorState _currentState = CursorState.Default;

    private enum CursorState
    {
        Default,
        Hover,
        Clicked
    }

    // �ʱ�ȭ �Լ��� Ŀ�� �̹����� ����
    public static void Initialize()
    {
        // ResourceManager �ν��Ͻ��� ���� Ŀ�� �̹��� �ε�
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");
        _hoverCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-on");

        // �ε� ���� �� ��� �α� ���
        if (_defaultCursor == null)
        {
            Debug.LogWarning("Default cursor not found at: Art/Image/cursor-off");
        }

        if (_hoverCursor == null)
        {
            Debug.LogWarning("Hover cursor not found at: Art/Image/cursor-on");
        }

        SetCursor();
    }

    public static void AddCursorChangeEvents(GameObject target)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        // ���콺�� ������Ʈ�� �ö��� �� Ŀ���� hoverCursor�� ����
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnPointerEnter());
        trigger.triggers.Add(pointerEnter);

        // ���콺�� ������Ʈ�� ����� �� �⺻ Ŀ���� ����
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnPointerExit());
        trigger.triggers.Add(pointerExit);

        // ��ư Ŭ�� �� �⺻ Ŀ���� ����
        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => OnClick());
        trigger.triggers.Add(pointerClick);
    }


    private static void OnPointerEnter()
    {
        _currentState = CursorState.Hover; // ���¸� Hover�� ����
        SetCursor();
    }

    private static void OnPointerExit()
    {
        // ���콺�� Ŭ���� ���°� �ƴ� ���� Ŀ���� Default�� ����
        if (_currentState != CursorState.Clicked)
        {
            _currentState = CursorState.Default; // ���¸� Default�� ����
            SetCursor();
        }
    }

    private static void OnClick()
    {
        _currentState = CursorState.Clicked; // ���¸� Clicked�� ����
        SetCursor();
    }
    private static void SetCursor()
    {
        switch (_currentState)
        {
            case CursorState.Hover:
                Texture2D resizedHoverCursor = ResizeTexture(_hoverCursor, 128, 128); // ȣ�� Ŀ�� ũ�� ����
                Cursor.SetCursor(resizedHoverCursor, _cursorHotspot, CursorMode.ForceSoftware);
                break;

            case CursorState.Clicked:
            case CursorState.Default:
            default:
                Texture2D resizedDefaultCursor = ResizeTexture(_defaultCursor, 128, 128); // �⺻ Ŀ�� ũ�� ����
                Cursor.SetCursor(resizedDefaultCursor, _cursorHotspot, CursorMode.ForceSoftware);
                break;
        }
    }
    public static void UpdateCursor()
    {
        // ���� ���콺 ��ġ�� �����´�
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // ��ư ���̾� üũ (�±׸� ���)
        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("Button"))
        {
            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition))
            {
                _currentState = CursorState.Hover; // ȣ�� ���·� ����
                SetCursor();
                return; // ��ư ���� ���� ���� �� �̻� üũ���� ����
            }
        }

        // ���̾� 7 üũ
        int targetMask = (1 << 7); // ���̾� ����ũ (7�� ���̾ Target���� ����)
        if (Physics.Raycast(ray, out hit, 100.0f, targetMask))
        {
            GameObject targetObj = hit.collider.gameObject;
            if (targetObj != null)
            {
                _currentState = CursorState.Hover; // ȣ�� ���·� ����
                SetCursor();
                return; // ���̾� 7 ���� ���� ���� �� �̻� üũ���� ����
            }
        }

        // ���콺�� �ƹ� ��ư�̳� ���̾� 7 ���� ������ �⺻ Ŀ�� ���·� ���ư�
        if (_currentState != CursorState.Default)
        {
            _currentState = CursorState.Default;
            SetCursor();
        }
    }
    private static Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        Texture2D resizedTexture = new Texture2D(width, height);
        Color[] pixels = originalTexture.GetPixels();
        Color[] resizedPixels = resizedTexture.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int originalX = Mathf.FloorToInt(x * ((float)originalTexture.width / width));
                int originalY = Mathf.FloorToInt(y * ((float)originalTexture.height / height));
                resizedPixels[y * width + x] = originalTexture.GetPixel(originalX, originalY);
            }
        }

        resizedTexture.SetPixels(resizedPixels);
        resizedTexture.Apply();
        return resizedTexture;
    }
}
