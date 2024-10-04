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

    // ���� ���� �����ϴ� �Լ�
    public void SetNextScene(Define.Scene nextScene)
    {
        _nextScene = nextScene;
        Init();
    }

    // Skip ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void OnSkipButtonClicked()
    {
        PlayButtonSound();
        if (_nextScene != Define.Scene.Unknown)  // Define.Scene.Unknown�� ���ǵ��� ���� �⺻��
        {
            Managers.Scene.LoadScene(_nextScene);  // ������ ���� ������ ��ȯ
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
