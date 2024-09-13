using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_User : UI_Popup
{
    CameraController _cameraController;

    enum Buttons
    {
        NameButton,
        HistoryButton,
        CloseButton,
        HistoryBookButton,
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
        _cameraController = FindObjectOfType<CameraController>();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        GetText((int)Texts.HistoryText).text = Managers.Data.userInfo.title;
        GetText((int)Texts.NameText).text = Managers.Data.userInfo.name;
        GetButton((int)Buttons.HistoryButton).gameObject.BindEvent(OnButtonClicked);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        GetButton((int)Buttons.HistoryBookButton).gameObject.BindEvent(OnHistoryBookButtonClicked);
    }

    public void OnButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_History>();
    }

    public void OnCloseButtonClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
        _cameraController.MoveToInitialPositionAndRotation();
    }

    public void OnHistoryBookButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_History_Book>();
    }
}
