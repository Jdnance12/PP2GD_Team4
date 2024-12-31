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

    [Header("---- Modifiers ----")]
    public float upgradeMultiplier = 1.25f;
    private const int maxUpgrades = 4;

    public int playerHealthCost;
    public int shieldCost;
    public int gunDmgCost;
    public int bladeDmgCost;
    public int hookDistCost;

    public int playerHealthUpCount;
    public int shieldUpCount;
    public int gunDmgUpCount;
    public int bladeDmgUpCount;
    public int hookDistUpCount;

    [Header("---- Stats ----")]
    public int upgradedPlayerHP;
    public int upgradedShield;
    public int upgradedGunDamage;
    public int upgradedBladeDamage;
    public int upgradedHookDistance;

    public int playerCurrency;

    private bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        gm = gameManager.instance;

        //Base Stats
        upgradedPlayerHP = gm.playerBaseHP;
        upgradedShield = gm.shieldBaseAmount;
        upgradedGunDamage = gm.gunBaseDamage;
        upgradedBladeDamage = gm.bladeBaseDamage;
        upgradedHookDistance = gm.hookBaseDistance;

        //Base Costs
        playerHealthCost = 1;
        //Base Counts

    }

    private void OnTriggerEnter(Collider other)
    {
        //Enter the Menu
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            //uiElement.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //uiElement.SetActive(false);
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
        if (playerCurrency >= playerHealthCost && playerHealthUpCount <= 4)
        {
            
            //Update Player Currency
            SubtractPartsCount(playerHealthCost);

            //Upgrade Cost
            playerHealthCost += playerHealthCost;

            //Health Up
            upgradedPlayerHP = Mathf.RoundToInt(upgradedPlayerHP * upgradeMultiplier);
            playerHealthUpCount++;

            gm.player.GetComponent<Player_Controller>().UpdateMaxHP(GetUpgradedHealth());
        }
    }

    public void ShieldUpgrade()
    {
        if(playerCurrency >= shieldCost && shieldUpCount <= 4)
        {
            SubtractPartsCount(shieldCost);

            shieldCost += shieldCost;

            upgradedShield = Mathf.RoundToInt(upgradedShield + 100f);

            gm.player.GetComponent<Player_Controller>().UpdateMaxShield(GetUpgradedShield());
        }
    }

    public void GunDmgUpgrade()
    {
        if(playerCurrency >= gunDmgCost && gunDmgUpCount <= 4)
        {
            
            //Update Player Currency
            SubtractPartsCount(gunDmgCost);

            //Upgrade Cost
            gunDmgCost += gunDmgCost;

            //Damage Up
            upgradedGunDamage = Mathf.RoundToInt(upgradedGunDamage * upgradeMultiplier);
            gunDmgUpCount++;
            
        }
    }

    public void BladeDmgUpgrade()
    {
        if (playerCurrency >= bladeDmgCost && bladeDmgUpCount <= 4)
        {
            
            //Update Player Currency
            SubtractPartsCount(bladeDmgCost);

            //Upgrade Cost
            bladeDmgCost += bladeDmgCost;

            //Damage Up
            upgradedBladeDamage = Mathf.RoundToInt(upgradedBladeDamage * upgradeMultiplier);
            bladeDmgUpCount++;
            
        }
    }
    public float GetUpgradedHealth()
    {
        return upgradedPlayerHP;
    }
    public float GetUpgradedGunDamage()
    {
        return upgradedGunDamage;
    }
    public float GetUpgradedBladeDamage()
    {
        return upgradedBladeDamage;
    }
    public float GetUpGradedHookDistance()
    {
        return upgradedHookDistance;
    }
    public float GetUpgradedShield()
    {
        return upgradedShield;
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
        //gunDmgCostText.text = gunDmgCost.ToString("F0");
        //bladeDmgCostText.text = bladeDmgCost.ToString("F0");
        //hookDistCostText.text = hookDistCost.ToString("F0");
    }
}
