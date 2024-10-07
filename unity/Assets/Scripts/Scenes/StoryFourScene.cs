using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFourScene : BaseScene
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

        SceneType = Define.Scene.StoryFour;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.ShipView);

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(16.3f, 16.8f, -12f), new Vector3(11.3734999f, 14.1086664f, -7.5930295f),
             new Quaternion(-0.11f, 0.54f, -0.07f, -0.83f), new Quaternion(-0.118602261f, 0.704953134f, -0.121421814f, -0.688644588f),
              1.3f);
        Managers.Sound.Play("RpgGameBGM/WarTheme(Loop)", Define.Sound.Bgm, 1.3f);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StoryFourScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(4);
    }
}
