using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterstellarManager : MonoBehaviour {

    private bool interstellarRun;
    public float destinationCameraFov;
    public float accelerationSpeed = 1;
    public float accelerationMultiplier = 0.1f;
    
    // Update is called once per frame
    void LateUpdate () {
        if(SceneLoader.instance != null)
        {
            if (interstellarRun)
            {
                ManipulateCamera();

                if (Mathf.Approximately(Camera.main.fieldOfView, destinationCameraFov))
                {
                    interstellarRun = false;
                    SceneLoader.instance.PlayLoadingScene();
                }
            }

            // when scene was loaded from scene with interstellar shader

            if (SceneLoader.instance.stopLoadingScene)
            {
                ManipulateCamera();
            }
        }
    }

    private void ManipulateCamera()
    {
        float fov = Mathf.MoveTowards(Camera.main.fieldOfView, destinationCameraFov, Time.deltaTime * accelerationSpeed);
        Camera.main.fieldOfView = fov;
        accelerationSpeed += accelerationMultiplier;
    }

    public void InterstellarMode()
    {
        interstellarRun = true;
    }

}
