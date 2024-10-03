using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TurtleShipSign : MonoBehaviour, IInteractable
{
    UI_User popup = null;
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
            popup = Managers.UI.ShowPopupUI<UI_User>();
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
