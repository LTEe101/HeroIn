using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // GameObject에 T 타입의 컴포넌트가 있으면 반환하고, 없으면 새로 추가하여 반환하는 함수
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        // T 타입의 컴포넌트를 GameObject에서 찾음
        T component = go.GetComponent<T>();
        // 컴포넌트가 없으면 새로 추가
        if (component == null)
            component = go.AddComponent<T>();
        // 컴포넌트를 반환
        return component;
    }

    // 자식 오브젝트 중에서 이름으로 특정 GameObject를 찾는 함수
    // recursive가 true면 재귀적으로 하위 오브젝트까지 찾음
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        // Transform 컴포넌트를 찾아서 가져옴
        Transform transform = FindChild<Transform>(go, name, recursive);
        // Transform을 찾지 못했으면 null 반환
        if (transform == null)
            return null;

        // Transform이 있으면 해당 GameObject 반환
        return transform.gameObject;
    }

    // 자식 오브젝트 중에서 이름으로 특정 T 타입 컴포넌트를 찾는 함수
    // recursive가 true면 하위 오브젝트까지 모두 탐색
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        // GameObject가 null이면 반환
        if (go == null)
            return null;

        // 재귀적 탐색이 아닌 경우
        if (recursive == false)
        {
            // 직속 자식들만 순회하며 이름이 일치하거나 이름이 없으면 컴포넌트를 찾음
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    // T 타입의 컴포넌트를 찾아 반환
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        // 재귀적으로 모든 자식들을 탐색
        else
        {
            // 자식 오브젝트들에서 T 타입의 컴포넌트를 찾아 반환
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        // 해당하는 컴포넌트를 찾지 못했을 경우 null 반환
        return null;
    }
}
