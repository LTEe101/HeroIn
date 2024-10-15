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
        VolumeBar // 슬라이더 컴포넌트
    }

    [SerializeField] private Sprite volumeOnSprite;  // 볼륨 켜진 상태 이미지
    [SerializeField] private Sprite volumeOffSprite; // 볼륨 꺼진 상태 이미지
    private bool isVolumeOn = true; // 현재 볼륨 상태 (true: 볼륨 켜짐, false: 음소거)
    private Image volumeButtonImage; // VolumeButton의 이미지 컴포넌트
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
        volumeButtonImage = GetButton((int)Buttons.VolumeOnOff).GetComponent<Image>(); // VolumeButton의 Image 컴포넌트 가져오기
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
        // 음소거 상태를 토글하고, 상태에 맞게 버튼 이미지 변경
        Managers.Sound.ToggleMute();

        isVolumeOn = !Managers.Sound.IsMuted(); // SoundManager에서 음소거 상태를 가져옴
        SetVolumeState(isVolumeOn);
    }

    // 볼륨 상태 설정 함수
    private void SetVolumeState(bool isVolumeOn)
    {
        this.isVolumeOn = isVolumeOn;

        // 현재 볼륨 상태에 따라 버튼 이미지 변경
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
        Managers.Sound.SetVolume(value); // SoundManager의 음량 조절
    }
}
