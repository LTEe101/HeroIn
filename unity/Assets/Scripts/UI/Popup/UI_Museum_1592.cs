using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Museum_1592 : UI_Popup
{
    enum Buttons
    {
        CloseButton,
        EnterButton,
    }

    enum Texts
    {
        DescriptionText,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts)); 

        // 버튼 이벤트 등록
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        GetButton((int)Buttons.EnterButton).gameObject.BindEvent(OnEnterButtonClicked);
        GetTMP((int)Texts.DescriptionText).text = "역사에 대한 설명 데이터";
    }

    // CloseButton 이벤트
    void OnCloseButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }

    // EnterButton 이벤트
    void OnEnterButtonClicked(PointerEventData data)
    {
        // 메타버스 입장
        Debug.Log("메타버스 입장");
    }
}
