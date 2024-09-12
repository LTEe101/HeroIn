using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_User : UI_Popup
{ 
    enum Buttons
    {
        NameButton,
        HistoryButton,
        CloseButton,
    }

    enum Texts
    {
        NameText,
        HistoryText,
    }

    enum Images
    {
        ProfileImg,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        GetText((int)Texts.HistoryText).text = Managers.Data.userInfo.title;
        GetText((int)Texts.NameText).text = Managers.Data.userInfo.name;
        GetButton((int)Buttons.HistoryButton).gameObject.BindEvent(OnButtonClicked);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
    }

    public void OnButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_History>();
    }

    public void OnCloseButtonClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }
}
