using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Skip : UI_Scene
{
    private Define.Scene _nextScene;
    enum Buttons
    {
        SkipButton
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.SkipButton).gameObject, (PointerEventData data) => { OnSkipButtonClicked(); }, Define.UIEvent.Click);
    }

    // 다음 씬을 설정하는 함수
    public void SetNextScene(Define.Scene nextScene)
    {
        _nextScene = nextScene;
        Init();
    }

    // Skip 버튼 클릭 시 호출되는 함수
    private void OnSkipButtonClicked()
    {
        PlayButtonSound();
        if (_nextScene != Define.Scene.Unknown)  // Define.Scene.Unknown는 정의되지 않은 기본값
        {
            Managers.Scene.LoadScene(_nextScene);  // 설정된 다음 씬으로 전환
        }
        else
        {
            Debug.LogWarning("Next scene is not set.");
        }
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
