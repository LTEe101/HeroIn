using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFiveScene : BaseScene
{
    private CameraController _cameraController;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;

    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.StoryFive;
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-314.2f, 46.6f, -39.6f), new Vector3(-372.6f, 16.49f, 81.44f),
             new Quaternion(-0.111f, 0.47f, -0.06f, -0.87f), new Quaternion(-0.13f, 0.6f, -0.1f, -0.78f),
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
