using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class StoryThreeScene : BaseScene
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

        SceneType = Define.Scene.StoryThree;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.GameOne);

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(16.3f, 16.8f, -12f), new Vector3(12.4f, 14.1f, -9.1f),
             new Quaternion(-0.11f, 0.54f, -0.07f, -0.83f), new Quaternion(-0.08f, 0.53f, -0.05f, -0.84f),
              1.2f);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StoryThreeScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(3);
    }
}
