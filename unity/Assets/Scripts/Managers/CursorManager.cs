using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D _defaultCursor;  // 기본 커서 이미지
    [SerializeField]
    private Texture2D _hoverCursor;
    private int cursorWidth = 128;
    private int cursorHeight = 128;
    private enum CursorState
    {
        Default,
        Hover,
        Clicked
    }
    [SerializeField]
    private CursorState _currentState = CursorState.Default;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // 기본 커서를 로드
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");
        _hoverCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-on");
        // 기본 커서를 적용
        if (_defaultCursor != null)
        {
            ApplyCursor(_defaultCursor);
        }
        else
        {
            Debug.LogWarning("Default cursor not found at: Art/Image/cursor-off");
        }
        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;

    }
    private void Update()
    {
        // 매 프레임마다 InputManager의 업데이트 함수 호출
        Managers.Input.OnUpdate();
        UpdateCursor();
    }
    // 마우스 이벤트 처리를 위한 함수
    private void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.PointerEnter:
                // 호버 커서를 적용
                if (_hoverCursor != null)
                {
                    ApplyCursor(_hoverCursor);
                    _currentState = CursorState.Hover;
                }
                break;

            case Define.MouseEvent.Click:
            case Define.MouseEvent.PointerExit:
                // 기본 커서를 적용
                if (_currentState != CursorState.Clicked && _defaultCursor != null)
                {
                    ApplyCursor(_defaultCursor);
                    _currentState = CursorState.Default;
                }
                break;

          
        }
    }
    // 버튼 태그 및 레이어 7 체크하여 커서를 변경하는 함수
    private void UpdateCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // 버튼 태그가 있는 오브젝트 위에 있을 때
        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("Button"))
        {
            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition))
            {
                ApplyCursor(_hoverCursor);  // 버튼 위에 있으면 호버 커서로 변경
                _currentState = CursorState.Hover;
                return;  // 버튼 위에 있으면 더 이상 체크하지 않음
            }
        }

        GameObject hoveredObject = Managers.Input.HoveredObject;

        // 호버된 객체가 있는지 확인
        if (hoveredObject != null)
        {
            // 버튼 태그가 있는 객체이거나, 레이어 7의 객체이면 호버 커서로 변경
            if ( hoveredObject.layer == 7)
            {
                ApplyCursor(_hoverCursor);  // 호버 커서로 변경
                _currentState = CursorState.Hover;
                return;  // 더 이상 체크할 필요 없음
            }
        }

        // 위 조건이 충족되지 않으면 기본 커서로 변경
        if (_currentState != CursorState.Default)
        {
            ApplyCursor(_defaultCursor);
            _currentState = CursorState.Default;
        }
    }
    private void ApplyCursor(Texture2D cursorTexture)
    {
        Texture2D resizedCursor = ResizeTexture(cursorTexture, cursorWidth, cursorHeight);
        Cursor.SetCursor(resizedCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        Texture2D resizedTexture = new Texture2D(width, height);
        Color[] pixels = originalTexture.GetPixels();
        Color[] resizedPixels = resizedTexture.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 크기를 비율로 조정하여 새로운 픽셀 색상을 설정
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
