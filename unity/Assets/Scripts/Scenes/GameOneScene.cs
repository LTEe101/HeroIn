using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOneScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.GameOne;
        UI_Game_Desc popup = Managers.UI.ShowPopupUI<UI_Game_Desc>();
        popup.LoadInfos(1);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("GameOneScene Clear!");
    }
}
