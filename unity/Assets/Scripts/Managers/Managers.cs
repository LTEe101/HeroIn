using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    APIManager _api;
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    PoolManager _pool = new PoolManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    LoadingManager _loading;
    CursorManager _cursor;
    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static APIManager API { get { return Instance._api; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static LoadingManager Loading { get { return Instance._loading; } }
    public static CursorManager Cursor { get { return Instance._cursor; } }
    void Start()
    {
        Init();
	}

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            s_instance._api = go.AddComponent<APIManager>();
            s_instance._loading = go.AddComponent<LoadingManager>();
            s_instance._cursor = go.AddComponent<CursorManager>();
            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
        }		
	}

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
