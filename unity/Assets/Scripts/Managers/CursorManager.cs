using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D _defaultCursor;  // �⺻ Ŀ�� �̹���
    [SerializeField]
    private Texture2D _hoverCursor;
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
        // �⺻ Ŀ���� �ε�
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");
        _hoverCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-on");
        // �⺻ Ŀ���� ����
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
        // �� �����Ӹ��� InputManager�� ������Ʈ �Լ� ȣ��
        Managers.Input.OnUpdate();
        UpdateCursor();
    }
    // ���콺 �̺�Ʈ ó���� ���� �Լ�
    private void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.PointerEnter:
                // ȣ�� Ŀ���� ����
                if (_hoverCursor != null)
                {
                    ApplyCursor(_hoverCursor);
                    _currentState = CursorState.Hover;
                }
                break;

            case Define.MouseEvent.Click:
            case Define.MouseEvent.PointerExit:
                // �⺻ Ŀ���� ����
                if (_currentState != CursorState.Clicked && _defaultCursor != null)
                {
                    ApplyCursor(_defaultCursor);
                    _currentState = CursorState.Default;
                }
                break;

          
        }
    }
    // ��ư �±� �� ���̾� 7 üũ�Ͽ� Ŀ���� �����ϴ� �Լ�
    private void UpdateCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // ��ư �±װ� �ִ� ������Ʈ ���� ���� ��
        foreach (GameObject buttonObj in GameObject.FindGameObjectsWithTag("Button"))
        {
            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition))
            {
                ApplyCursor(_hoverCursor);  // ��ư ���� ������ ȣ�� Ŀ���� ����
                _currentState = CursorState.Hover;

                rt.localScale = Vector3.one * 1.1f;
                return;  // ��ư ���� ������ �� �̻� üũ���� ����
            }
            else
            {
                // ���콺�� ��ư ������ ����� ���� ũ��� �ǵ���
                rt.localScale = Vector3.one;  // �⺻ ũ��� ����
            }
        }

        GameObject hoveredObject = Managers.Input.HoveredObject;

        // ȣ���� ��ü�� �ִ��� Ȯ��
        if (hoveredObject != null)
        {
            // ���̾� 7�� ��ü�̸� ȣ�� Ŀ���� ����
            if ( hoveredObject.layer == 7 || hoveredObject.name == "Exit")
            {
                ApplyCursor(_hoverCursor);  // ȣ�� Ŀ���� ����
                _currentState = CursorState.Hover;
                return;  // �� �̻� üũ�� �ʿ� ����
            }
        }

        // �� ������ �������� ������ �⺻ Ŀ���� ����
        if (_currentState != CursorState.Default)
        {
            ApplyCursor(_defaultCursor);
            _currentState = CursorState.Default;
        }
    }
    private void ApplyCursor(Texture2D cursorTexture)
    {
        Texture2D resizedCursor = cursorTexture;
        Cursor.SetCursor(resizedCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

}
