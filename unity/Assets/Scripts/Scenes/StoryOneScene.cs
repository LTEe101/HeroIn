using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class StoryOneScene : BaseScene
{
    private CameraController _cameraController;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;
    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.StoryOne;
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(48f, 31f, -19.2f), new Vector3(70.1f, 24.7f, -42.6f),
             new Quaternion(0.05f, 0.94f, -0.15f, 0.3f), new Quaternion(0.03f, 0.96f, -0.15f, 0.22f),
              1.2f);
        Managers.Sound.Play("AsianBGM/AsiaTensionBGM_01", Define.Sound.Bgm);
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("StoryOneScene Clear!");
    }
    void OnCloseUpComplete()
    {
        UI_Story popup = Managers.UI.ShowPopupUI<UI_Story>();
        popup.LoadDialogs(1);
        
    }
}
