using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySixScene : BaseScene
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

        SceneType = Define.Scene.StorySix;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.AfterStory);


        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(58.6f, 27.8f, -23.5f), new Vector3(64.8626022f, 13.6136112f, -44.7812691f),
             new Quaternion(-0.089f, 0.84f, -0.14f, -0.5f), new Quaternion(-0.108274944f, 0.646892965f, -0.0935138166f, -0.749040246f),
              1.2f);
        Managers.Sound.Play("RpgGameBGM/StrangeRoad(Loop)", Define.Sound.Bgm, 1.4f);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StorySixScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(6);
    }
}
