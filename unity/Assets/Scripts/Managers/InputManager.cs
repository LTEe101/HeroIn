using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    // ���콺 ȣ�� ���� ��ü�� �����ϱ� ���� ����
    private GameObject _hoveredObject = null;

    public GameObject HoveredObject
    {
        get { return _hoveredObject; } // ȣ���� ��ü�� ��ȯ�ϴ� �Ӽ�
    }

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Ű �Է��� ���� �� ó��
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        // ���콺 �̺�Ʈ ó��
        if (MouseAction != null)
        {
            // ���콺 Ŭ�� ó��
            if (Input.GetMouseButtonDown(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Click);
            }

            // ���콺 ȣ�� ó��
            HandleMouseHover();
        }
    }

    void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast�� ����Ͽ� ���콺�� � ��ü ���� �ִ��� Ȯ��
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // ���콺�� ���ο� ��ü ���� ���� ��
            if (_hoveredObject != hitObject)
            {
                // ������ ȣ���� ��ü�� ������ ȣ�� ���� ó��
                if (_hoveredObject != null)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerExit);
                }

                // ���Ӱ� ȣ���� ��ü ó��
                _hoveredObject = hitObject;
                MouseAction.Invoke(Define.MouseEvent.PointerEnter);
            }
        }
        else
        {
            // ���콺�� � ��ü ������ ���� ��
            if (_hoveredObject != null)
            {
                MouseAction.Invoke(Define.MouseEvent.PointerExit);
                _hoveredObject = null;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        _hoveredObject = null;
    }
}
