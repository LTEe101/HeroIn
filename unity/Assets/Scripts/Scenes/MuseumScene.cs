using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MuseumScene : BaseScene
{
    private Vector3 originalScale; // 객체의 원래 Scale을 저장할 변수
    public float highlightScaleFactor = 1.02f; // 하이라이트 시 커질 비율

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Museum;

        // InputManager의 MouseAction에 이벤트 등록
        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;
    }

    void Update()
    {
    }

    // 마우스 입력 처리 메서드
    void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Click:
                Debug.Log("마우스 클릭이 감지되었습니다.");
                OnMouseClicked();
                break;
            case Define.MouseEvent.PointerEnter:
                Debug.Log("마우스가 객체 위에 있습니다.");
                OnPointerEnter(Managers.Input.HoveredObject); // 현재 호버된 객체 밝게 하기
                break;
            case Define.MouseEvent.PointerExit:
                Debug.Log("마우스가 객체에서 벗어났습니다.");
                OnPointerExit(Managers.Input.HoveredObject); // 하이라이트 리셋
                break;
        }
    }

    // 하이라이트를 원래 상태로 복구하는 함수
    void ResetHighlight(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // 원래 Scale로 복구
            hoveredObject.transform.localScale = originalScale;
            hoveredObject = null; // 하이라이트 객체 초기화

        }
    }
    void ChangeOutlineColor(GameObject go, Color newColor)
    {
        // 오브젝트에서 Outline 컴포넌트를 가져옴
        Outline outline = go.GetComponent<Outline>();

        // Outline 컴포넌트가 있으면 색상 변경
        if (outline != null)
        {
            // 아웃라인 컬러 변경
            outline.OutlineColor = newColor;
        }
        else
        {
            Debug.LogWarning("Outline 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void OnPointerEnter(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        Debug.Log(hoveredObject.name);
        switch (hoveredObject.name)
        {
            case "Spaceship":
                // UI_Enter 활성화
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(true);
               
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "1592":
                Debug.Log("test");
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            default:
                break;

        }
    }
    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        switch (hoveredObject.name)
        {
            case "Spaceship":
                // UI_Enter 비활성화
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(false);
                
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "1592":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            default:
                break;

        }
    }
    // 마우스 호버 시 객체를 키우고 UI_Enter 출력하는 함수
    void HighlightObject(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // 원래 Scale 저장
            originalScale = hoveredObject.transform.localScale;

            // 하이라이트 Scale 적용
            hoveredObject.transform.localScale = originalScale * highlightScaleFactor; // Scale 증가
        }
    }

    // 마우스 클릭 시 호출되는 함수
    void OnMouseClicked()
    {
        GameObject currentHovered = Managers.Input.HoveredObject;

        if (currentHovered != null)
        {
            string hitObjectName = currentHovered.name;
            Debug.Log(hitObjectName);
            switch (hitObjectName)
            {
                case "Spaceship":
                    Managers.Scene.LoadScene(Define.Scene.Home);
                    break;
                case "1592":
                    Managers.UI.ShowPopupUI<UI_Museum_1592>();
                    break;
                default:
                    Debug.Log("해당 오브젝트에 대한 팝업이 없습니다.");
                    break;
            }
        }
    }

    public override void Clear()
    {
        // 씬이 종료될 때 호출되는 함수, 필요에 따라 추가 작업 가능
        Managers.Input.Clear(); // InputManager의 이벤트를 클리어
    }
}
