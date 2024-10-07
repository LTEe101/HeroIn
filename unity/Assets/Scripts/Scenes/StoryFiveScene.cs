using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFiveScene : BaseScene
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

        SceneType = Define.Scene.StoryFive;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.MotionGame);

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-314.2f, 46.6f, -39.6f), new Vector3(-373.64801f, 15.9368544f, 80.9988327f),
             new Quaternion(-0.111f, 0.47f, -0.06f, -0.87f), new Quaternion(-0.095823586f, 0.408768982f, -0.0432105586f, -0.906564176f),
              1.2f);
        
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StoryFiveScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(5);
    }
}
