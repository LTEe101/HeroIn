using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;

public class UI_Alert : UI_Popup, IInitializablePopup
{
    string message;
    enum Buttons
    {
        CheckButton,
    }
    enum Texts
    {
        DescriptionText,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        // 버튼 이벤트 등록
        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnCheckButtonClicked);

        GetTMP((int)Texts.DescriptionText).text = message;
    }

    // Initialize 메서드를 통해 파라미터를 받아서 처리
    public void Initialize(params object[] args)
    {
        if (args.Length > 0 && args[0] is string message)
        {
            this.message = message;
        }
    }

    // CheckButton 클릭 이벤트
    public void OnCheckButtonClicked(PointerEventData data)
    {
        ClosePopupUI(); // 팝업 닫기
    }
}
