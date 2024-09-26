using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionGameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.MotionGame;
        
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("MotionGameScene Clear!");
    }
}
