using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader> {

    public bool startLoadingScene = false;
    public bool stopLoadingScene = false;
    public float shaderDuration = 0;
    public string sceneToBeLoaded;
    public string sceneDuringLoad;
    
    public void PlayLoadingScene()
    {
        this.startLoadingScene = true;
        SceneManager.LoadSceneAsync(sceneDuringLoad);
    }
	
}
