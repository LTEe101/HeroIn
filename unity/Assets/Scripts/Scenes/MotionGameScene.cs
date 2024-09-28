using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionGameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.MotionGame;
        UI_Game_Desc popup = Managers.UI.ShowPopupUI<UI_Game_Desc>();
        popup.LoadInfos(2);

    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("MotionGameScene Clear!");
    }
}
