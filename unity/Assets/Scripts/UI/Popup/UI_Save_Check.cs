using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Save_Check : UI_Popup, IInitializablePopup
{
    enum Buttons
    {
        SaveButton,
        CancelButton,
    }

    enum Texts
    {
        SaveText,
    }

    UI_History _parentUI;  // 부모 UI 참조를 위한 변수

    // Initialize 메서드를 통해 메시지와 parentUI를 받아서 설정
    public void Initialize(params object[] args)
    {
        Init();
        Debug.Log(args[0]);
        Debug.Log(args[1]); 
        if (args != null && args.Length > 1)
        {
            string message = args[0] as string;  // 첫 번째 파라미터로 메시지를 받음
            _parentUI = args[1] as UI_History;   // 두 번째 파라미터로 parentUI 받음

            if (!string.IsNullOrEmpty(message))
            {
                GetText((int)Texts.SaveText).text = $"대표 업적을 { message }(으)로 설정하시겠습니까 ?";  // SaveText에 메시지를 설정
            }
        }
    }

    public override void Init()
    {
        base.Init();

        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        // 버튼 이벤트 등록
        GetButton((int)Buttons.SaveButton).gameObject.BindEvent(OnSaveButtonClicked);
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnCancelButtonClicked);
    }

    public void OnSaveButtonClicked(PointerEventData data)
    {
        Debug.Log("대표 업적 저장");

        Managers.UI.ClosePopupUI(this);
        _parentUI.OnCloseButtonClicked(data);
    }

    public void OnCancelButtonClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }
}
