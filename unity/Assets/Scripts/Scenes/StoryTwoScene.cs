using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class StoryTwoScene : BaseScene
{
    private CameraController _cameraController;
    private UI_Skip _skipUI;
    float _rotationSpeed = 1.5f;
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
        _cameraController.StartCloseUp(new Vector3(-359.9f, 29f, -25f), new Vector3(-374.501434f, 20.0523167f, 2.59323335f),
             new Quaternion(-0.14f, 0.36f, -0.05f, -0.92f), new Quaternion(-0.136936754f, -0.0521296784f, 0.00721441302f, -0.989180863f),
              1.2f);
        
    }

    private void Update()
    {
        float rotation = _rotationSpeed * Time.time;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
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
