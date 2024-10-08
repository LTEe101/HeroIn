using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    void Awake()
	{
        int width = 1920;  // 가로 해상도
        int height = 1080; // 세로 해상도

        // 전체 화면 모드에서 원하는 해상도로 설정
        Screen.SetResolution(width, height, true);
        Init();
	}

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        Managers.UI.ShowSceneUI<UI_Setting_Icon>();
    }

    public abstract void Clear();
}
