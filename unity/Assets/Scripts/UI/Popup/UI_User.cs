using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_User : UI_Popup
{
    [SerializeField]
    private Vector3 _initialPosition = new Vector3(-10f, 1.910354f, -2.25f);

    [SerializeField]
    private Quaternion _initialRotation = Quaternion.Euler(6f, 0.0f, 0.0f); // 초기 회전

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
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        GetText((int)Texts.HistoryText).text = Managers.Data.userInfo.title;
        GetText((int)Texts.NameText).text = Managers.Data.userInfo.name;
        GetImage((int)Images.ProfileImg).sprite = Managers.Data.userInfo.imgNo;
        GetButton((int)Buttons.HistoryButton).gameObject.BindEvent(OnButtonClicked);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        GetButton((int)Buttons.HistoryBookButton).gameObject.BindEvent(OnHistoryBookButtonClicked);
    }

    public void OnButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        Managers.UI.ShowPopupUI<UI_History>();
    }

    public void OnCloseButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        Managers.UI.ClosePopupUI(this);
        Camera.main.transform.position = _initialPosition;   // 카메라를 원래 위치로 되돌림
        Camera.main.transform.rotation = _initialRotation;
    }

    public void OnHistoryBookButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        Managers.UI.ShowPopupUI<UI_History_Book>();
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
