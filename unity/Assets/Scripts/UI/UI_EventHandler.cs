using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI_EventHandler 클래스는 IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler 인터페이스를 구현하여 클릭, 드래그, 마우스 오버 이벤트를 처리하는 스크립트입니다.
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // 클릭 이벤트를 처리할 델리게이트 (Action)
    public Action<PointerEventData> OnClickHandler = null;

    // 드래그 이벤트를 처리할 델리게이트 (Action)
    public Action<PointerEventData> OnDragHandler = null;

    // 마우스 오버(Enter) 이벤트를 처리할 델리게이트 (Action)
    public Action<PointerEventData> OnPointerEnterHandler = null;

    // 마우스 나감(Exit) 이벤트를 처리할 델리게이트 (Action)
    public Action<PointerEventData> OnPointerExitHandler = null;

    // 마우스 클릭 시 호출되는 함수 (IPointerClickHandler 인터페이스에서 제공)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 핸들러가 등록되어 있을 경우, 해당 핸들러를 실행
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    // 드래그 시 호출되는 함수 (IDragHandler 인터페이스에서 제공)
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 핸들러가 등록되어 있을 경우, 해당 핸들러를 실행
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    // 마우스가 UI 위로 올라갔을 때 호출되는 함수 (IPointerEnterHandler 인터페이스에서 제공)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버 핸들러가 등록되어 있을 경우, 해당 핸들러를 실행
        if (OnPointerEnterHandler != null)
            OnPointerEnterHandler.Invoke(eventData);
    }

    // 마우스가 UI에서 벗어났을 때 호출되는 함수 (IPointerExitHandler 인터페이스에서 제공)
    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 나감 핸들러가 등록되어 있을 경우, 해당 핸들러를 실행
        if (OnPointerExitHandler != null)
            OnPointerExitHandler.Invoke(eventData);
    }
}
