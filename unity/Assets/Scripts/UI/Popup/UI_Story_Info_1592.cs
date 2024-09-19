using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Story_Info_1592 : UI_Popup
{
    private GameObject mainTv; // main_tv 게임 오브젝트 참조
    private RectTransform uiRectTransform; // UI의 RectTransform 참조

    private void Start()
    {
        mainTv = GameObject.Find("main_tv"); // main_tv 게임 오브젝트 찾기
        Init();
    }

    public override void Init()
    {
        base.Init();

        // UI 위치를 main_tv에 맞추기
        if (mainTv != null)
        {
            uiRectTransform = GetComponent<RectTransform>();
            uiRectTransform.SetParent(mainTv.transform); // main_tv의 자식으로 설정
            uiRectTransform.localScale = Vector3.one; // 스케일 설정

            // main_tv의 위치에 맞춰 UI 위치 조정
            uiRectTransform.anchoredPosition = Vector2.zero; // 부모에 맞춰 UI 위치 조정
        }

        // 추가적인 UI 설정 (예: UI 요소 초기화 등)
        // 예: UI 텍스트, 이미지 설정 등
        // Text uiText = Util.FindChild<Text>(mainTv, "UIElementName");
        // if (uiText != null) uiText.text = "내용 설정";
    }
}
