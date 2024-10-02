using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ShipViewScene : BaseScene
{
    private Animator _animator;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.ShipView;
        _animator = GetComponent<Animator>();
    }

    public void OnAnimationEnd()
    {
        Managers.Scene.LoadScene(Define.Scene.StoryFive);
    }

    public override void Clear()
    {
        Debug.Log("ShipViewScene Clear!");
        
    }
}
