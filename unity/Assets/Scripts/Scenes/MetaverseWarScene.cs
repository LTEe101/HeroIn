using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaverseWarScene : BaseScene
{
    float _rotationSpeed = 1.5f;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.MetaverseWar;
        
    }

    private void Update()
    {
        float rotation = _rotationSpeed * Time.time;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }

    public override void Clear()
    {
        Debug.Log("MetaverseWarScene Clear!");
    }
}
