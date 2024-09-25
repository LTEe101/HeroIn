using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    // 마우스 호버 중인 객체를 추적하기 위한 변수
    private GameObject _hoveredObject = null;

    public GameObject HoveredObject
    {
        get { return _hoveredObject; } // 호버된 객체를 반환하는 속성
    }

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 키 입력이 있을 때 처리
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        // 마우스 이벤트 처리
        if (MouseAction != null)
        {
            // 마우스 클릭 처리
            if (Input.GetMouseButtonDown(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Click);
            }

            // 마우스 호버 처리
            HandleMouseHover();
        }
    }

    void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast를 사용하여 마우스가 어떤 객체 위에 있는지 확인
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // 마우스가 새로운 객체 위에 있을 때
            if (_hoveredObject != hitObject)
            {
                // 이전에 호버된 객체가 있으면 호버 나감 처리
                if (_hoveredObject != null)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerExit);
                }

                // 새롭게 호버된 객체 처리
                _hoveredObject = hitObject;
                MouseAction.Invoke(Define.MouseEvent.PointerEnter);
            }
        }
        else
        {
            // 마우스가 어떤 객체 위에도 없을 때
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
