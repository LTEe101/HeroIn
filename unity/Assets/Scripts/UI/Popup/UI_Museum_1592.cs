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
        GetTMP((int)Texts.DescriptionText).text = "임진왜란의 현장을 생생하게 만나보세요!\n\n임진왜란 속 중요한 장면들을 재미있게 탐험하며 우리 역사를 배워볼 수 있어요.\n\n함께 역사 속으로 떠나볼까요?";
    }

    // CloseButton 이벤트
    void OnCloseButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        ClosePopupUI();
    }

    // EnterButton 이벤트
    void OnEnterButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        Managers.Scene.LoadScene(Define.Scene.MetaverseWar);
        // 메타버스 입장
        Debug.Log("메타버스 입장");
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
