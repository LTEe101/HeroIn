using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class CursorEventHelper
{
    private static Texture2D _defaultCursor;
    private static Texture2D _hoverCursor;
    private static Vector2 _cursorHotspot = new Vector2(0, 0);  // 커서 중심점
    private static int _cursorWidth = 128;    // 커서 너비
    private static int _cursorHeight = 128;   // 커서 높이

    private static CursorState _currentState = CursorState.Default;

    private enum CursorState
    {
        Default,
        Hover,
        Clicked
    }

    // 초기화 함수로 커서 이미지를 설정
    public static void Initialize()
    {
        // ResourceManager 인스턴스를 통해 커서 이미지 로드
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");
        _hoverCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-on");

        // 로드 실패 시 경고 로그 출력
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

        // 마우스가 오브젝트에 올라갔을 때 커서를 hoverCursor로 변경
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnPointerEnter());
        trigger.triggers.Add(pointerEnter);

        // 마우스가 오브젝트를 벗어났을 때 기본 커서로 변경
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnPointerExit());
        trigger.triggers.Add(pointerExit);

        // 버튼 클릭 시 기본 커서로 변경
        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => OnClick());
        trigger.triggers.Add(pointerClick);
    }


    private static void OnPointerEnter()
    {
        _currentState = CursorState.Hover; // 상태를 Hover로 변경
        SetCursor();
    }

    private static void OnPointerExit()
    {
        // 마우스가 클릭한 상태가 아닐 때만 커서를 Default로 변경
        if (_currentState != CursorState.Clicked)
        {
            _currentState = CursorState.Default; // 상태를 Default로 변경
            SetCursor();
        }
    }

    private static void OnClick()
    {
        _currentState = CursorState.Clicked; // 상태를 Clicked로 변경
        SetCursor();
    }
    private static void SetCursor()
    {
        switch (_currentState)
        {
            case CursorState.Hover:
                Texture2D resizedHoverCursor = ResizeTexture(_hoverCursor, 128, 128); // 호버 커서 크기 조정
                Cursor.SetCursor(resizedHoverCursor, _cursorHotspot, CursorMode.ForceSoftware);
                break;

            case CursorState.Clicked:
            case CursorState.Default:
            default:
                Texture2D resizedDefaultCursor = ResizeTexture(_defaultCursor, 128, 128); // 기본 커서 크기 조정
                Cursor.SetCursor(resizedDefaultCursor, _cursorHotspot, CursorMode.ForceSoftware);
                break;
        }
    }
    public static void UpdateCursor()
    {
        // 현재 마우스 위치를 가져온다
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // 버튼 레이어 체크 (태그를 사용)
        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("Button"))
        {
            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition))
            {
                _currentState = CursorState.Hover; // 호버 상태로 변경
                SetCursor();
                return; // 버튼 위에 있을 때는 더 이상 체크하지 않음
            }
        }

        // 레이어 7 체크
        int targetMask = (1 << 7); // 레이어 마스크 (7번 레이어가 Target임을 가정)
        if (Physics.Raycast(ray, out hit, 100.0f, targetMask))
        {
            GameObject targetObj = hit.collider.gameObject;
            if (targetObj != null)
            {
                _currentState = CursorState.Hover; // 호버 상태로 변경
                SetCursor();
                return; // 레이어 7 위에 있을 때는 더 이상 체크하지 않음
            }
        }

        // 마우스가 아무 버튼이나 레이어 7 위에 없으면 기본 커서 상태로 돌아감
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
