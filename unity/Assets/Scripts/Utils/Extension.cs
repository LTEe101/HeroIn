using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    // GameObject에 T 타입의 컴포넌트를 추가하거나 이미 존재하는 경우 해당 컴포넌트를 반환하는 함수
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        // Util 클래스의 GetOrAddComponent 함수를 호출하여 컴포넌트를 반환
        return Util.GetOrAddComponent<T>(go);
    }

    // GameObject에 특정 이벤트(클릭 또는 드래그)를 바인딩하는 확장 메서드
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // UI_Base 클래스의 BindEvent 함수를 호출하여 이벤트를 바인딩
        UI_Base.BindEvent(go, action, type);
    }
}
