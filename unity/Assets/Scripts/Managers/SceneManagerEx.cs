using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

	public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
    }

    public void LodingLoadScene(Define.Scene type)
    {
        // 로딩 씬으로 전환
        LoadScene(Define.Scene.Loading);

        // 로딩 씬에서 실제 씬을 비동기적으로 로드
        Managers.Loading.LoadScene(GetSceneName(type));
    }


    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }


}
