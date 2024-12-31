using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("---- Components ----")]
    private GameManager gm;
    public GameObject gunWeapon;
    public GameObject bladeWeapon;
    public GameObject empSkill;

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
    public int doubleJumpCost;

    public int playerHealthUpCount;
    public int shieldUpCount;
    public int gunDmgUpCount;
    public int bladeDmgUpCount;
    public int hookDistUpCount;
    public int doubleJumpCount;

    [Header("---- Stats ----")]
    public float upgradedPlayerHP;
    public float upgradedShield;
    public float upgradedGunDamage;
    public float upgradedBladeDamage;
    public float upgradedHookDistance;

    public int playerCurrency;

    private bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        gunWeapon = GameObject.Find("Gun Weapon Object");
        bladeWeapon = GameObject.Find("Blade Weapon Object");
        empSkill = GameObject.Find("EMP Weapon Object");

        // Base Stats
        upgradedPlayerHP = gm.player.GetComponent<PlayerController>().maxHP;
        upgradedShield = gm.player.GetComponent <PlayerController>().maxShield;
        upgradedGunDamage = gunWeapon.GetComponent<GunWeapon>().damage;
        upgradedBladeDamage = bladeWeapon.GetComponent<BladeWeapon>().damage;

        // Base Cost
        playerHealthCost = 50;
        shieldCost = 50;
        gunDmgCost = 50;
        bladeDmgCost = 50;
        doubleJumpCost = 100;

        // Base Counts
        playerHealthUpCount = 0;
        shieldUpCount = 0; 
        gunDmgUpCount = 0;
        bladeDmgUpCount = 0;
        doubleJumpCount = 0;
        
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



    //Skill Unlocks
    public void DoubleJumpUnlock()
    {
        if(playerCurrency <= doubleJumpCost && doubleJumpCount < 1)
        {
            gm.player.GetComponent<PlayerController>().jumpMax = 2;
        }
    }
    
    
    
    //Stat Upgrades
    public void HealthUpgrade()
    {
        if(playerCurrency >= playerHealthCost && playerHealthUpCount <= 4)
        {
            SubtractPartsCount(playerHealthCost);

            playerHealthCost += playerHealthCost;

            upgradedPlayerHP = Mathf.RoundToInt(upgradedPlayerHP * upgradeMultiplier);
            playerHealthUpCount++;

            gm.player.GetComponent<PlayerController>().UpdateMaxHP(GetUpgradedHealth());
        }
    }
    public void ShieldUpgrade()
    {
        if (playerCurrency >= shieldCost && shieldUpCount <= 4)
        {
            SubtractPartsCount(shieldCost);

            shieldCost += shieldCost;

            upgradedShield = Mathf.RoundToInt(upgradedShield + 100f);
            shieldUpCount++;

            gm.player.GetComponent<PlayerController>().UpdateMaxShield(GetUpgradedShield());
        }
    }
    public void GunDmgUpgrade()
    {
        if (playerCurrency >= gunDmgCost && gunDmgUpCount <= 4)
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
    public float GetUpgradedShield()
    {
        return upgradedShield;
    }
    public float GetUpgradedGunDamage()
    {
        return upgradedGunDamage;
    }
    public float GetUpgradedBladeDamage()
    {
        return upgradedBladeDamage;
    }
}
