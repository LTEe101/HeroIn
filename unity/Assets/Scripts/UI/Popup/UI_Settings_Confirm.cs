using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Settings_Confirm : UI_Popup
{
    enum Buttons
    {
        ConfirmButton,
        CancelButton
    }
    enum Texts
    {
        ConfirmText
    }
    private Define.ConfirmType _confirmType;
    
    public void InitConfirmType(Define.ConfirmType confirmType)
    {
        _confirmType = confirmType;
        Init();
        Text confirmText = Get<Text>((int)Texts.ConfirmText);
        if (_confirmType == Define.ConfirmType.Home)
        {
            confirmText.text = "Ȩ���� ���ư��ðڽ��ϱ�?";  // Home�� ���� Ȯ�� �޽���
        }
        else if (_confirmType == Define.ConfirmType.Exit)
        {
            confirmText.text = "�����Ͻðڽ��ϱ�?";  // Exit�� ���� Ȯ�� �޽���
        }
        
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        if (_confirmType == Define.ConfirmType.Home)
        {
            BindEvent(GetButton((int)Buttons.ConfirmButton).gameObject, (PointerEventData data) => { PlayButtonSound(); Managers.Scene.LoadScene(Define.Scene.Home); }, Define.UIEvent.Click);
        } else if (_confirmType == Define.ConfirmType.Exit)
        {
            BindEvent(GetButton((int)Buttons.ConfirmButton).gameObject, (PointerEventData data) => { PlayButtonSound(); Application.Quit(); }, Define.UIEvent.Click);
        }
        
        BindEvent(GetButton((int)Buttons.CancelButton).gameObject, (PointerEventData data) => { PlayButtonSound(); ClosePopupUI(); }, Define.UIEvent.Click);

    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
