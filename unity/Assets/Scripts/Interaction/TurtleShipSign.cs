using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TurtleShipSign : MonoBehaviour, IInteractable
{
    UI_Meta_Info popup = null;
    public bool _isPress { get; private set; }

    void Start()
    {
        _isPress = false;

    }

    public string GetInteractText()
    {
        return "[G] 버튼을 누르세요.";
    }

    public void Interact()
    {
       
        if (!_isPress)
        {
            Debug.Log("상호작용");
            popup = Managers.UI.ShowPopupUI<UI_Meta_Info>();
            popup.InitMetaHistory(1);
            _isPress = true;
        }
        else
        {
            popup.ClosePopupUI(); 
            _isPress = false;
        }
    }

   
    void Update()
    {

    }
}
