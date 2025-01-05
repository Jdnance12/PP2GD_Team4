using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    public GameObject uiElement;

    public int upgradeCost;
    //public GameObject gunWeaponObj;
    //public GunWeapon gunWeapon;
    public GameObject upgradeMenu;

    public bool playerInRange = false;


    private void Start()
    {
        upgradeMenu = GameObject.Find("Upgrade Menu");
        upgradeMenu.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uiElement.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiElement.SetActive(false);
        }
    }

    private void Update()
    {
        if(playerInRange && Input.GetKey(KeyCode.T))
        {
            OpenUpgradeMenu();
        }
        //if (upgradeMenu.activeSelf)
        //{
        //    if(Input.GetKeyDown(KeyCode.T))
        //    {
        //        CloseUpgradeMenu();
        //    }
        //}
    }

    private void OpenUpgradeMenu()
    {
        GameManager.instance.menuActive = upgradeMenu;
        GameManager.instance.menuActive.gameObject.SetActive(true);
        GameManager.instance.GamePaused();
    }
    public void CloseUpgradeMenu()
    {
        //upgradeMenu.SetActive(false);
        GameManager.instance.GameUnPaused();
    }
}
