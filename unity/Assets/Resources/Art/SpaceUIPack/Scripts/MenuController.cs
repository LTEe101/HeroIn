using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject MenuButtons; // assign the buttons menu to this in inspector
    public GameObject CharacterManu; // assign the character menu to this in inspector
    public GameObject WeaponMenu; // assign the Weapon menu to this in inspector
    public GameObject InventoryMenu; // assign the character menu to this in inspector

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CharacterOnClick() {
        CharacterManu.SetActive(true);
        MenuButtons.SetActive(true);
        WeaponMenu.SetActive(false);
        InventoryMenu.SetActive(false);
    }
    public void weaponOnClick()
    {
        CharacterManu.SetActive(false);
        MenuButtons.SetActive(true);
        WeaponMenu.SetActive(true);
        InventoryMenu.SetActive(false);
    }
    public void InventoryOnClick()
    {
        CharacterManu.SetActive(false);
        MenuButtons.SetActive(false);
        WeaponMenu.SetActive(false);
        InventoryMenu.SetActive(true);
    }
    public void InventoryClose()
    {
        CharacterManu.SetActive(true);
        MenuButtons.SetActive(true);
        WeaponMenu.SetActive(false);
        InventoryMenu.SetActive(false);
    }
}
