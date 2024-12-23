using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("--- Player Elements ----")]
    public GameObject player; // Player object reference
    public playerController playerScript;

    [Header("--- UI Elements ----")]
    [SerializeField] public GameObject menuActive; // Current active menu
    [SerializeField] public GameObject menuPause; // Pause menu object
    [SerializeField] public GameObject menuWin, menuLose; //Win Lose Menu's
    [SerializeField] public GameObject weaponMenu;

    [SerializeField] public Image gunReticule;
    [SerializeField] public Image bladeReticule;

    float timeScaleOriginal;

    [Header("---- Bools ----")]
    public bool isPaused;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) // Check for pause/unpause input
        {
            if (menuActive == null) // No active menu
            {
                statePause(); // Enter pause state
                menuActive = menuPause; // Set pause menu
                menuActive.SetActive(true); // Show menu
            }
            else if (menuActive == menuPause) // Pause menu active
            {
                stateUnpause(); // Unpause game
            }
        }
    }

    public void statePause()
    {
        Debug.Log("Game paused.");
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        playerScript.enabled = false;
    }

    public void stateUnpause()
    {
        Debug.Log("Game unpaused.");
        isPaused = false;

        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerScript != null)
        {
            playerScript.enabled = true;
            playerScript.ResetPlayerState(); // Only reset shooting
        }

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void ShowWeaponMenu()
    {
        weaponMenu.SetActive(true);
    }
    public void HideWeaponMenu()
    {
        weaponMenu.SetActive(false);
    }
}
