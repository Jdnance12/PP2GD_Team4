using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{

    public int upgradeCost;
    public GunWeapon gunWeapon;
    public GameObject upgradeMenu;

    private bool playerInRange = false;

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            OpenUpgradeMenu();
        }
    }

    private void OpenUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        gameManager.instance.statePause();
    }
    public void CloseUpgradeMenu()
    {
        upgradeMenu.SetActive(false);
        gameManager.instance.stateUnpause();
    }
}
