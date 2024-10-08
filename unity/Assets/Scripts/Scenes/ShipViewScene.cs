using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ShipViewScene : BaseScene
{
    private Animator _animator;
    private UI_Skip _skipUI;
    float _rotationSpeed = 1.5f;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.ShipView;
        _animator = GetComponent<Animator>();

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.StoryFive);

    }
    private void Update()
    {
        float rotation = _rotationSpeed * Time.time;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
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
