using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    // UI 오브젝트들을 저장할 딕셔너리
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    // 추상 메서드, 상속받은 클래스에서 반드시 구현해야 함
    public abstract void Init();

    // 특정 타입(T)의 UI 요소들을 enum 이름을 기준으로 찾아서 바인딩하는 함수
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // enum의 이름들을 배열로 받아옴
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        // 딕셔너리에 타입이 이미 존재하는지 확인
        if (_objects.ContainsKey(typeof(T)))
        {
            Debug.LogWarning($"Type {typeof(T)} is already bound.");
            return;
        }
        // 해당 타입(T)의 오브젝트 배열을 딕셔너리에 저장
        _objects.Add(typeof(T), objects);

        // enum에서 이름을 하나씩 순회하며 오브젝트를 찾음
        for (int i = 0; i < names.Length; i++)
        {
            // 타입이 GameObject일 경우
            if (typeof(T) == typeof(GameObject))
                // 이름으로 자식 오브젝트를 찾음
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                // 그 외 타입일 경우 해당 컴포넌트를 자식 오브젝트에서 찾음
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            // 만약 찾지 못했다면 경고 메시지 출력
            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    // 저장된 UI 요소 중 특정 인덱스의 요소를 가져오는 함수
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        // 딕셔너리에서 해당 타입(T)의 오브젝트 배열을 찾음
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            // 없으면 null 반환
            return null;

        // 배열에서 해당 인덱스의 오브젝트를 T 타입으로 변환하여 반환
        return objects[idx] as T;
    }

    // GameObject를 특정 인덱스로 가져오는 함수
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    // Text 컴포넌트를 특정 인덱스로 가져오는 함수
    protected Text GetText(int idx) { return Get<Text>(idx); }
    // Button 컴포넌트를 특정 인덱스로 가져오는 함수
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    // Image 컴포넌트를 특정 인덱스로 가져오는 함수
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    // 특정 게임 오브젝트에 이벤트를 바인딩하는 함수
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // 게임 오브젝트에 UI_EventHandler 컴포넌트를 추가하거나 가져옴
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        // 이벤트 타입에 따라 클릭 또는 드래그 이벤트를 바인딩
        switch (type)
        {
            case Define.UIEvent.Click:
                // 기존에 등록된 클릭 이벤트를 제거하고 새로 추가
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                // 기존에 등록된 드래그 이벤트를 제거하고 새로 추가
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
