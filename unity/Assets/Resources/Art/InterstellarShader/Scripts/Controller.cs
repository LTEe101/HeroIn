using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
        // to load another scene while shader is running
        // Scene loader must have been initialized from previous scene 
        if (SceneLoader.instance != null && SceneLoader.instance.startLoadingScene == true)
        {
            Invoke("ChangeLevel", SceneLoader.instance.shaderDuration);
        }
    }

    void ChangeLevel()
    {
        SceneLoader.instance.stopLoadingScene = true;
        SceneLoader.instance.startLoadingScene = false;
        SceneManager.LoadSceneAsync(SceneLoader.instance.sceneToBeLoaded);
    }
    
}
