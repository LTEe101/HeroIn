using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class StoryOneScene : BaseScene
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

        SceneType = Define.Scene.StoryOne;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.StoryTwo);

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(69.6947021f, 28.1520462f, -28.1233196f), new Vector3(64.2851181f, 13.381629f, -45.2922134f),
             new Quaternion(-0.0737237707f, 0.925995588f, -0.273118824f, -0.250006258f), new Quaternion(0.0380175672f, -0.702807605f, 0.0376910046f, 0.709362745f),
              1.0f);
       
    }

    private void Update()
    {
        float rotation = _rotationSpeed * Time.time;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
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
