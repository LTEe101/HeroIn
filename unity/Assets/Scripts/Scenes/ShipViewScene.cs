using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ShipViewScene : BaseScene
{
    private Animator _animator;
    private UI_Skip _skipUI;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.ShipView;
        _animator = GetComponent<Animator>();

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.StoryFive);

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
