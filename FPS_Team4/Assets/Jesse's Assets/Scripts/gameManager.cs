using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("--- Player Elements ----")]
    //Player Components
    public GameObject player;
    public playerController playerScript;
    // Player HP
    public Image playerHPBar;
    public GameObject playerDamageQue;

    [Header("--- UI Elements ----")]
    //Paused Menus
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuWin, menuLose;
    [SerializeField] public TMP_Text scrapCountText;
    [SerializeField] public TMP_Text nodeCountText;
    float timeScaleOriginal;
    //Weapons And Reticlules
    [SerializeField] public GameObject weaponMenu;
    [SerializeField] public Image gunReticule;
    [SerializeField] public Image bladeReticule;
    //Item Additions
    public int scrapCount;
    public int nodeCount;

    [Header("---- Bools ----")]
    public bool isPaused;

    private void Awake()
    {
        instance = this;

        menuActive = null;

        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player"); // Find player by tag
        playerScript = player.GetComponent<playerController>(); // Get player script
    }

    private void Update()
    {

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
        //menuActive.SetActive(false);
        //menuActive = null;
    }

    public void youLose()
    {

    }

    public void ShowWeaponMenu()
    {
        weaponMenu.SetActive(true);
    }
    public void HideWeaponMenu()
    {
        weaponMenu.SetActive(false);
    }
    public void AddScrapCount(int amount)
    {
        scrapCount += amount;
        scrapCountText.text = scrapCount.ToString("F0");
    }
    public bool SpendScrap(int amount)
    {
        if(scrapCount >= amount)
        {
            scrapCount -= amount;
            return true;
        }
        return false;
    }
    public void AddNodeCount(int amount)
    {
        nodeCount += amount;
        nodeCountText.text = nodeCount.ToString("F0");
    }
}
