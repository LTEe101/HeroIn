using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaverseWarScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.MetaverseWar;
        
    }

    private void Update()
    {
        
    }

    public override void Clear()
    {
        Debug.Log("MetaverseWarScene Clear!");
    }
}
