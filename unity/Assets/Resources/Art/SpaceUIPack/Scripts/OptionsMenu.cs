using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

    public GameObject audiobox; // ad audiobox to this gameobject in inspector
    public GameObject videobox;// ad videobox to this gameobject in inspector
    // Start is called before the first frame update
    void Start()
    {
        videobox.SetActive(false);// set video box to false

        audiobox.SetActive(false);// set video box to false
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void audioTrigger() {
        audiobox.SetActive(true); // set audio box to true
        videobox.SetActive(false);// set video box to false
    }
    public void videoTrigger() {
        audiobox.SetActive(false);// set video box to false
        videobox.SetActive(true);// set audio box to true
    }
    public void Exit()
    {
        videobox.SetActive(false);// set video box to false
        audiobox.SetActive(false);// set video box to false

    }
}
