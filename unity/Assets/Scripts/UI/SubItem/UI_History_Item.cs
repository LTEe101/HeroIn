using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_History_Item : UI_Base
{
    enum GameObjects
    {
        UI_History_Item,
        HistoryItemNameText,
    }

    string _name;
    UI_History _parentUI;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.HistoryItemNameText).GetComponent<Text>().text = _name;

        Get<GameObject>((int)GameObjects.UI_History_Item).BindEvent((PointerEventData) => {

            Managers.UI.ShowPopupUI<UI_Save_Check>(args: new object[] { _name, _parentUI });
        });
    }

    public void SetInfo(string name, UI_History parentUI)
    {
        _name = name;
        _parentUI = parentUI;
    }
}
