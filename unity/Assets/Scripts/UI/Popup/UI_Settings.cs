using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : UI_Popup
{
    enum Buttons
    {
        CloseButton,
        ExitButton,
        HomeButton,
        VolumeOnOff
    }
    enum Sliders
    {
        VolumeBar // �����̴� ������Ʈ
    }

    [SerializeField] private Sprite volumeOnSprite;  // ���� ���� ���� �̹���
    [SerializeField] private Sprite volumeOffSprite; // ���� ���� ���� �̹���
    private bool isVolumeOn = true; // ���� ���� ���� (true: ���� ����, false: ���Ұ�)
    private Image volumeButtonImage; // VolumeButton�� �̹��� ������Ʈ
    private Slider volumeSlider;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        volumeButtonImage = GetButton((int)Buttons.VolumeOnOff).GetComponent<Image>(); // VolumeButton�� Image ������Ʈ ��������
        volumeSlider = Get<Slider>((int)Sliders.VolumeBar);
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
        BindEvent(GetButton((int)Buttons.CloseButton).gameObject, (PointerEventData data) => { PlayButtonSound(); Managers.UI.CloseSelectPopupUI(this); }, Define.UIEvent.Click);
        
        BindEvent(GetButton((int)Buttons.ExitButton).gameObject, (PointerEventData data) => { 
            PlayButtonSound();
            UI_Settings_Confirm popup = Managers.UI.ShowPopupUI<UI_Settings_Confirm>();
            popup.InitConfirmType(Define.ConfirmType.Exit);
        }, Define.UIEvent.Click);
        
        BindEvent(GetButton((int)Buttons.HomeButton).gameObject, (PointerEventData data) => { 
            PlayButtonSound();
            UI_Settings_Confirm popup = Managers.UI.ShowPopupUI<UI_Settings_Confirm>();
            popup.InitConfirmType(Define.ConfirmType.Home);
        }, Define.UIEvent.Click);
        
        BindEvent(GetButton((int)Buttons.VolumeOnOff).gameObject, (PointerEventData data) => { PlayButtonSound(); ToggleVolume(); }, Define.UIEvent.Click);

        SetVolumeState(!Managers.Sound.IsMuted());
        volumeSlider.value = Managers.Sound.GetVolume();
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
    private void ToggleVolume()
    {
        // ���Ұ� ���¸� ����ϰ�, ���¿� �°� ��ư �̹��� ����
        Managers.Sound.ToggleMute();

        isVolumeOn = !Managers.Sound.IsMuted(); // SoundManager���� ���Ұ� ���¸� ������
        SetVolumeState(isVolumeOn);
    }

    // ���� ���� ���� �Լ�
    private void SetVolumeState(bool isVolumeOn)
    {
        this.isVolumeOn = isVolumeOn;

        // ���� ���� ���¿� ���� ��ư �̹��� ����
        if (isVolumeOn)
        {
            volumeButtonImage.sprite = volumeOnSprite;
        }
        else
        {
            volumeButtonImage.sprite = volumeOffSprite;
        }
    }
    private void AdjustVolume(float value)
    {
        Managers.Sound.SetVolume(value); // SoundManager�� ���� ����
    }
}
