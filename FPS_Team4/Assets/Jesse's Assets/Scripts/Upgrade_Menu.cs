using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Upgrade_Menu : MonoBehaviour
{
    [Header("---- Components ----")]
    private gameManager gm;
    public GameObject player;
    public GameObject uIElement;
    public Player_Controller playerCtrlr;
    public GunWeapon gunScript;
    public BladeWeapon bladeScript;
    public GrappleHookController grappleHookScript;

    [Header("---- Text Components ----")]
    public TMP_Text playerHealthCostText;
    public TMP_Text gunDmgCostText;
    public TMP_Text bladeDmgCostText;
    public TMP_Text hookDistCostText;
    public TMP_Text playerCurrencyText;

    [Header("---- Buttons ----")]
    public Button gunDmgUpButton;
    public Button bladeDmgUpButton;
    public Button hookDistUpButton;

    [Header("---- States ----")]
    public int playerHP;
    public int playerHealthUpCount;
    public int playerHealthCost;

    public int gunDamage;
    public int gunDmgUpCount;
    public int gunDmgCost;

    public int bladeDamage;
    public int bladeDmgUpCount;
    public int bladeDmgCost;

    public int hookDist;
    public int hookDistUpCount;
    public int hookDistCost;

    public int playerCurrency;

    private bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        gm = gameManager.instance;
        player = gm.player;
        playerCtrlr = gm.playerScript;
        playerCurrency = gm.partsCount;

        //Base Stats
        playerHP = 10;
        gunDamage = 2;
        bladeDamage = 5;
        hookDist = 100;

        //Base Costs
        playerHealthCost = 1;
        gunDmgCost = 1;
        bladeDmgCost = 1;
        hookDistCost = 1;

        //Base Counts
        playerHealthUpCount = 0;
        gunDmgUpCount = 0;
        bladeDmgUpCount = 0;
        hookDistUpCount = 0;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Enter the Menu
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uIElement.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uIElement.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmounts();
        
        //Open the Menu
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                gm.ShowUpgradeMenu();
            }
        }

        //Exit the Menu
        if (Input.GetButtonDown("Cancel"))
        {
            gm.stateUnpause();
        }
    }

    public void HealthUpgrade()
    {
        if (playerCurrency >= playerHealthCost)
        {
            if (playerHealthUpCount <= 4)
            {
                //Update Player Currency
                SubtractPartsCount(playerHealthCost);

                //Upgrade Cost
                playerHealthCost += playerHealthCost;

                //Health Up
                playerHP += playerHP;
                playerHealthUpCount++;
            }
        }
    }

    public void GunDmgUpgrade()
    {
        if(playerCurrency >= gunDmgCost)
        {
            if(gunDmgUpCount <= 4)
            {
                //Update Player Currency
                SubtractPartsCount(gunDmgCost);

                //Upgrade Cost
                gunDmgCost += gunDmgCost;

                //Damage Up
                gunDamage += gunDamage;
                gunDmgUpCount++;
            }
        }
    }

    public void BladeDmgUpgrade()
    {
        if (playerCurrency >= bladeDmgCost)
        {
            if (gunDmgUpCount <= 4)
            {
                //Update Player Currency
                SubtractPartsCount(bladeDmgCost);

                //Upgrade Cost
                bladeDmgCost += bladeDmgCost;

                //Damage Up
                bladeDamage += bladeDamage;
                bladeDmgUpCount++;
            }
        }
    }

    public void AddPartsCount(int amount)
    {
        playerCurrency += amount;
        playerCurrencyText.text = playerCurrency.ToString("F0");
    }

    public void SubtractPartsCount(int amount)
    {
        playerCurrency -= amount;
        playerCurrencyText.text = playerCurrency.ToString("F0");
    }

    public void UpdateAmounts()
    {
        playerHealthCostText.text = playerHealthCost.ToString("F0");
        gunDmgCostText.text = gunDmgCost.ToString("F0");
        bladeDmgCostText.text = bladeDmgCost.ToString("F0");
        hookDistCostText.text = hookDistCost.ToString("F0");
    }
}
