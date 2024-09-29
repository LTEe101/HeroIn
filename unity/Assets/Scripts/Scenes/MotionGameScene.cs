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