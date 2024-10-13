using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public interface IInitializablePopup
{
    void Initialize(params object[] args);
}

public class UIManager
{
    int _order = 10; // UI의 정렬 순서를 관리하기 위한 변수

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); // 팝업 UI를 관리하는 스택
    UI_Scene _sceneUI = null; // 현재 활성화된 씬 UI를 저장하는 변수

    // UI 루트 오브젝트를 가져오는 프로퍼티. 없으면 새로 생성.
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }
    public int GetStackCount()
    {
        return _popupStack.Count;
    }

    // 스택의 맨 위에 있는 팝업 UI를 반환하는 함수
    public UI_Popup GetTopPopupUI()
    {
        if (_popupStack.Count == 0)
            return null; // 스택이 비어 있으면 null 반환

        return _popupStack.Peek(); // 스택 최상단 팝업 반환
    }

    // UI 오브젝트에 Canvas를 추가하고 정렬 순서를 설정하는 함수
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go); // Canvas 컴포넌트가 없으면 추가
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // UI를 화면에 표시
        canvas.overrideSorting = true; // 정렬 순서 덮어쓰기 설정

        if (sort)
        {
            canvas.sortingOrder = _order; // 정렬 순서 설정
            _order++; // 다음 UI의 정렬 순서를 위해 증가
        }
        else
        {
            canvas.sortingOrder = 0; // 정렬을 원하지 않는 경우 기본값으로 설정
        }
    }

    // SubItem UI를 생성하고 부모 객체에 설정하는 함수
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name; // 이름이 없으면 타입 이름으로 설정

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}"); // 리소스를 통해 오브젝트 생성
        if (parent != null)
            go.transform.SetParent(parent); // 부모 객체가 있으면 부모로 설정

        return Util.GetOrAddComponent<T>(go); // 해당 UI 컴포넌트를 반환
    }

    // Scene UI를 생성하고 화면에 표시하는 함수
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name; // 이름이 없으면 타입 이름으로 설정

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}"); // 리소스를 통해 Scene UI 생성
        T sceneUI = Util.GetOrAddComponent<T>(go); // UI_Scene 타입 컴포넌트 추가
        _sceneUI = sceneUI; // 현재 Scene UI로 설정

        go.transform.SetParent(Root.transform); // UI 루트 오브젝트 아래에 설정

        return sceneUI; // 생성된 Scene UI 반환
    }

    // Popup UI를 생성하고 화면에 표시하며, 파라미터를 넘기는 함수
    public T ShowPopupUI<T>(string name = null, object[] args = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}"); // 리소스를 통해 Popup UI 생성
        T popup = Util.GetOrAddComponent<T>(go); // UI_Popup 타입 컴포넌트 추가
        _popupStack.Push(popup); // 생성된 팝업을 스택에 저장

        go.transform.SetParent(Root.transform); // UI 루트 오브젝트 아래에 설정

        if (args != null && popup is IInitializablePopup initializablePopup)
        {
            initializablePopup.Initialize(args); // 넘겨받은 파라미터로 팝업 초기화
        }

        return popup;
    }

    // 특정 팝업을 닫는 함수
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return; // 스택에 팝업이 없으면 리턴

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!"); // 닫으려는 팝업이 스택의 최상단이 아니면 실패
            return;
        }

        ClosePopupUI(); // 최상단 팝업 닫기
    }

    // 최상단 팝업을 닫는 함수
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return; // 스택에 팝업이 없으면 리턴

        UI_Popup popup = _popupStack.Pop(); // 스택에서 팝업 제거
        Managers.Resource.Destroy(popup.gameObject); // 팝업 오브젝트 파괴
        popup = null; // 팝업 참조 해제
        _order--; // 팝업이 닫혔으므로 정렬 순서 감소
    }

    // 모든 팝업을 닫는 함수
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI(); // 팝업이 있을 때까지 계속 닫기
    }

    public void CloseSelectPopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return; // 스택에 팝업이 없으면 리턴

        // 팝업이 스택의 최상단에 있지 않더라도 닫을 수 있도록 변경
        if (_popupStack.Contains(popup))
        {
            Stack<UI_Popup> tempStack = new Stack<UI_Popup>();

            // 스택에서 닫으려는 팝업을 찾을 때까지 임시 스택에 이동
            while (_popupStack.Count > 0)
            {
                UI_Popup topPopup = _popupStack.Pop();
                if (topPopup == popup)
                {
                    Managers.Resource.Destroy(popup.gameObject); // 팝업 오브젝트 파괴
                    popup = null; // 팝업 참조 해제
                    _order--; // 정렬 순서 감소
                    break;
                }
                else
                {
                    tempStack.Push(topPopup); // 팝업이 아니면 임시 스택에 추가
                }
            }

            // 나머지 팝업을 원래 스택에 다시 넣음
            while (tempStack.Count > 0)
            {
                _popupStack.Push(tempStack.Pop());
            }
        }
        else
        {
            Debug.Log("Popup not found in the stack!"); // 스택에 팝업이 없으면 경고 메시지 출력
        }
    }
    // UI 관련 데이터를 모두 초기화하는 함수
    public void Clear()
    {
        CloseAllPopupUI(); // 모든 팝업 닫기
        _sceneUI = null; // 씬 UI 참조 해제
    }
}
