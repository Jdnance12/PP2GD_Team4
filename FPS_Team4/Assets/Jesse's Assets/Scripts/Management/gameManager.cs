using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("---- Player Initial Stats ----")]
    [SerializeField] public int playerBaseHP;
    [SerializeField] public int shieldBaseAmount;
    [SerializeField] public int gunBaseDamage;
    [SerializeField] public int bladeBaseDamage;
    [SerializeField] public int hookBaseDistance;

    [Header("--- Player Elements ----")]
    //Player Components
    public GameObject player;
    public Player_Controller playerScript;
    public Upgrade_Menu upgradeMeu;
    public Image playerHPBar;
    public Image playerShieldBar;
    public GameObject playerDamageQue;
    public GameObject playerShieldHitQue;

    [Header("--- Object UI Elements ----")]
    [SerializeField] TMP_Text grappleUIText;
    [SerializeField] Image grappleUIImage;


    [Header("--- Camera UI Elements ----")]
    //Paused Menus
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuWin, menuLose;
    [SerializeField] public GameObject upgradeMenu;
    [SerializeField] public Upgrade_Menu upgradeMenuScript;
    [SerializeField] public TMP_Text partsCountText;
    [SerializeField] public TMP_Text nodeCountText;
    float timeScaleOriginal;
    //Weapons And Reticlules
    [SerializeField] public Image gunReticule;
    [SerializeField] public Image bladeReticule;
    //Item Additions
    public int partsCount;
    public int nodeCount;

    [Header("---- Bools ----")]
    public bool isPaused;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        menuActive = null;

        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player"); // Find player by tag
        playerScript = player.GetComponent<Player_Controller>(); // Get player script
    }

    private void Update()
    {
        partsCountText.text = upgradeMenuScript.playerCurrencyText.text;
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void youLose()
    {

    }
    public void ShowUpgradeMenu()
    {
        statePause();
        menuActive = upgradeMenu;
        menuActive.SetActive(true);
    }
    public void HideUpgradeMenu()
    {
        stateUnpause();
    }
    public void AddNodeCount(int amount)
    {
        nodeCount += amount;
        nodeCountText.text = nodeCount.ToString("F0");
    }
}
