using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Check_Alert : UI_Popup, IInitializablePopup
{
    public Action OnCheck; // 확인 버튼 클릭 시 실행될 이벤트
    string message;
    enum Buttons
    {
        CheckButton,
        CancelButton,
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
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnCancelButtonClicked);
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
        OnCheck?.Invoke(); // 확인 버튼 클릭 시 이벤트 실행
        ClosePopupUI(); // 팝업 닫기
    }

    // CancelButton 클릭 이벤트
    public void OnCancelButtonClicked(PointerEventData data)
    {
        ClosePopupUI(); // 팝업 닫기
    }
}
