using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Control : UI_Popup
{
    enum Buttons
    {
        CloseButton
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

        // 버튼 이벤트 등록
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        
    }

    // CloseButton 이벤트
    public void OnCloseButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        ClosePopupUI();
    }
    
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}