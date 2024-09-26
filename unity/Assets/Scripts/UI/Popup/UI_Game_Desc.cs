using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Game_Desc : UI_Popup
{
    enum Texts
    {
        TitleText,
        DescText,
    }

    enum Buttons
    {
        StartButton,
    }
    enum Images
    {
        DescPaper,
    }
    public void LoadInfos(int gameId)
    {
        Init();
        if (Managers.Data.GameInfos.TryGetValue(gameId, out GameInfo gameInfo))
        {
            Get<Text>((int)Texts.TitleText).GetComponent<Text>().text = gameInfo.title;       // 게임 제목 설정
            Get<Text>((int)Texts.DescText).GetComponent<Text>().text = gameInfo.desc;   // 게임 설명 설정
        }
        else
        {
            Debug.LogWarning($"GameInfo with ID {gameId} not found.");
        }
    }
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        CursorEventHelper.Initialize();

        CursorEventHelper.AddCursorChangeEvents(GetButton((int)Buttons.StartButton).gameObject);
        BindEvent(GetButton((int)Buttons.StartButton).gameObject, (PointerEventData data) => { 
            ClosePopupUI();
            Managers.UI.ShowSceneUI<UI_Game_Score>();
        }, Define.UIEvent.Click);
    }
}
