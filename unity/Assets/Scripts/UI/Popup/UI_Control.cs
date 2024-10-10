using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Control : UI_Popup
{
    enum Buttons
    {
        CloseButton
    }
    enum GameObjects
    {
        Toggle
    }
    private Toggle _toggle;
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        // Toggle 컴포넌트 가져오기
        _toggle = GetObject((int)GameObjects.Toggle).GetComponent<Toggle>();

        // 버튼 이벤트 등록
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        _toggle.onValueChanged.AddListener(OnToggleValueChanged); // Toggle의 값 변경 이벤트 등록
    }

    // CloseButton 이벤트
    public void OnCloseButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        ClosePopupUI();
    }

    // Toggle 상태가 바뀔 때 호출되는 이벤트
    public void OnToggleValueChanged(bool isOn)
    {
        PlayButtonSound();
        if (isOn)
        {
            // Toggle이 켜졌을 때
            Managers.Data.ControlPannel = true;
        }
        else
        {
            // Toggle이 꺼졌을 때
            Managers.Data.ControlPannel = false;
        }
    }


    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}