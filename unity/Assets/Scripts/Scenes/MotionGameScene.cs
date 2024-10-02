using System.Linq;
using UnityEngine;

public class MotionGameScene : BaseScene
{
    private bool isGameStarted = false;
    private UI_Game_Desc popup;
    private IMotionGameScript[] gameplayScripts;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.MotionGame;
        popup = Managers.UI.ShowPopupUI<UI_Game_Desc>();
        popup.LoadInfos(2);
        Managers.Sound.Play("KarugamoBGM/KBF_Battle_Nomal_04", Define.Sound.Bgm, 0.8f);
        popup.OnStartGame += StartGame;

        gameplayScripts = FindObjectsOfType<MonoBehaviour>().OfType<IMotionGameScript>().ToArray();
        DisableGameScripts();
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
    }

    public override void Clear()
    {
        Debug.Log("MotionGameScene Clear!");
    }
}
