using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScene : BaseScene
{ 
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Home;
   
        Managers.UI.ShowPopupUI<UI_User>();
        //Managers.UI.ShowPopupUI<UI_History_Book>();
    }

    public override void Clear()
    {

    }
}
