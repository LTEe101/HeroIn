using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Setting_Icon : UI_Scene
{
    enum Buttons
    {
        SettingButton
    }
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, (PointerEventData data) => { PlayButtonSound();  Managers.UI.ShowPopupUI<UI_Settings>(); }, Define.UIEvent.Click);
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
