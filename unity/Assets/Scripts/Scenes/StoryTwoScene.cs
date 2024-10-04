using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class StoryTwoScene : BaseScene
{
    private CameraController _cameraController;
    private UI_Skip _skipUI;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;

    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.StoryTwo;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.StoryThree);

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-359.9f, 29f, -25f), new Vector3(-373.6f, 20.35f, 1.111f),
             new Quaternion(-0.14f, 0.36f, -0.05f, -0.92f), new Quaternion(-0.156f, 0.17f, -0.027f, -0.97f),
              1.2f);
        
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StoryTwoScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(2);
    }
}
