using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class BeforeStoryScene : BaseScene
{
    public CameraController _cameraController;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;
       
    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.BeforeStory;
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-13.1f, 55.7f, -79.8f), new Vector3(34.9f, 26.5f, -59.3f),
            new Quaternion(-0.24f, -0.54f, 0.16f, -0.8f), new Quaternion(0.26f, -0.17f, 0.05f, 0.95f),
             1.0f);
        Managers.Sound.Play("AsianBGM/AsiaTensionBGM_01", Define.Sound.Bgm);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("BeforeStoryScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Paper popup = Managers.UI.ShowPopupUI<UI_Paper>();
        popup.InitStory(Define.StoryType.Before);
    }
}
