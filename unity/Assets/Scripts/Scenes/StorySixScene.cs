using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySixScene : BaseScene
{
    private CameraController _cameraController;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;

    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.StorySix;
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(58.6f, 27.8f, -23.5f), new Vector3(33.7f, 10f, -48.47f),
             new Quaternion(-0.089f, 0.84f, -0.14f, -0.5f), new Quaternion(-0.058f, 0.567f, -0.04f, -0.82f),
              1.2f);
        Managers.Sound.Play("RpgGameBGM/StrangeRoad(Loop)", Define.Sound.Bgm);
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
