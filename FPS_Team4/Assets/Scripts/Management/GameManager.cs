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

    public GameObject player;
    public PlayerController playerScript;

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

    [SerializeField] public GameObject menuMisc;
    [SerializeField] public TMP_Text miscTitleText;
    [SerializeField] public TMP_Text miscBodyText;

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

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

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
            if(menuActive == null)
            {
                GamePaused();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive.activeSelf)
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
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void YouWin()
    {

    }
    public void YouLose()
    {

    }
    public void WeaponMenuUnPaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
