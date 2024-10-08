using System.Linq;
using UnityEngine;

public class MotionGameScene : BaseScene
{
    private bool isGameStarted = false;
    private UI_Game_Desc popup;
    private UI_Skip _skipUI;
    private IMotionGameScript[] gameplayScripts;
    private VideoStreamer videoStreamer; // 웹캠 종료를 위한 VideoStreamer 참조 추가
    float _rotationSpeed = 1.5f;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.MotionGame;

        _skipUI = Managers.UI.ShowSceneUI<UI_Skip>();
        _skipUI.SetNextScene(Define.Scene.StorySix);

        popup = Managers.UI.ShowPopupUI<UI_Game_Desc>();
        popup.LoadInfos(2);
        Managers.Sound.Play("KarugamoBGM/KBF_Battle_Nomal_04", Define.Sound.Bgm, 0.6f);
        popup.OnStartGame += StartGame;

        gameplayScripts = FindObjectsOfType<MonoBehaviour>().OfType<IMotionGameScript>().ToArray();
        DisableGameScripts();

        videoStreamer = FindObjectOfType<VideoStreamer>(); // VideoStreamer 가져오기

    }
    private void Update()
    {
        float rotation = _rotationSpeed * Time.time;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
    private void DisableGameScripts()
    {
        foreach (var script in gameplayScripts)
        {
            script.IsEnabled = false;
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        Debug.Log("Game Started!");

        // UI_Game_Score UI에 게임 모드를 전달
        var gameScoreUI = Managers.UI.ShowSceneUI<UI_Game_Score>();

        gameScoreUI.SetGameMode(true);

        EnableGameScripts();
    }

    private void EnableGameScripts()
    {
        foreach (var script in gameplayScripts)
        {
            script.IsEnabled = true;
        }
        Managers.UI.ShowSceneUI<UI_Motion_State>();
    }

    public override async void Clear() // 비동기 메서드로 변경
    {
        Debug.Log("MotionGameScene Clear!");

        // 웹소켓 및 웹캠 종료
        if (videoStreamer != null)
        {
            await videoStreamer.CloseWebSocketAndStopCam(); // await로 비동기 작업 대기
        }
    }
}