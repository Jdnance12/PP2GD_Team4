using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---- Bools ----")]
    public bool isPaused;

    [Header("---- Game Objects ----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject progressManager;
    public Progression_Manager progressScript;

    public UpgradeManager upgradeScript;

    [Header("---- UI Feedback ----")]
    public GameObject playerHealthHitImage;
    public GameObject playerShieldHitImage;
    public Image playerHPBar;
    public Image playerShieldBar;

    [Header("---- UI Menus ----")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuStart;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuWin, menuLose;
    [SerializeField] public GameObject settingsMenu; // Reference to the settings menu

    [SerializeField] public GameObject aiDialogueTextImage;
    [SerializeField] public TMP_Text aiDialogueText;

    [SerializeField] public GameObject menuMisc;
    [SerializeField] public TMP_Text miscTitleText;
    [SerializeField] public TMP_Text miscBodyText;

    [SerializeField] public TMP_Text nodeAmountText;

    [SerializeField] public GameObject upgradeMenuObject;

    [Header("---- UI Weapon Reticules ----")]
    public Image gunReticule;
    public Image bladeReticule;

    float timeScaleOriginal;

    [Header("---- UI Counts ----")]
    private int enemyCount;
    public int partsCount;
    public int nodeCount;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Getting the player's object and script
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        // Getting the Progress Manager
        progressManager = GameObject.Find("Progression Manager");
        progressScript = progressManager.GetComponent<Progression_Manager>();

        // Getting the Upgrade Manager before setting it as false
        upgradeScript = GetComponent<UpgradeManager>();
        upgradeMenuObject.SetActive(false);

        menuActive = null;

        timeScaleOriginal = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                GamePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause && menuActive.activeSelf)
            {
                GameUnPaused();
            }
        }
    }

    public void ShowStartMenu()
    {
        menuActive = menuStart;
        menuActive.SetActive(true);
    }

    public void GamePaused()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void GameUnPaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void ShowSettingsMenu()
    {
        if (settingsMenu != null)
        {
            if (menuPause != null && menuPause.activeSelf)
            {
                menuPause.SetActive(false);
            }
            GamePaused();
            menuActive = settingsMenu;
            menuActive.SetActive(true);
        }
    }

    public void YouWin()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
        GamePaused();
    }

    public void YouLose()
    {
        menuActive = menuLose;
        menuActive.SetActive(true);
        GamePaused();
    }

    public void WeaponMenuUnPaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddNodeCount(int amount)
    {
        nodeCount += amount;
        nodeAmountText.text = nodeCount.ToString("F0");
    }
}
