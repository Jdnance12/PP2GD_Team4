using UnityEngine; // Unity engine base
using System.Collections; // Collection management
using System.Collections.Generic; // Advanced collections
using TMPro; // TextMeshPro for UI

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton reference

    public GameObject drone; // Drone reference

    [SerializeField] GameObject menuActive; // Current active menu
    [SerializeField] GameObject menuPause; // Pause menu object
    [SerializeField] GameObject menuWin, menuLose; // Win/Lose menus
    [SerializeField] TMP_Text activeThreatsText; // Goal count display

    public GameObject player; // Player object reference
    public playerController playerScript; // Player script reference
  

    public bool isPaused; // Pause state flag

    float timeScaleOriginal; // Original time scale
    int goalCount; // Track current goal count

    void Awake() // Initial setup
    {
        instance = this; // Set singleton instance
        timeScaleOriginal = Time.timeScale; // Save original time scale
        player = GameObject.FindWithTag("Player"); // Find player by tag
        playerScript = player.GetComponent<playerController>(); // Get player script
        

        DroneAI[] drones = FindObjectsOfType<DroneAI>(); // Find all drones
        foreach (DroneAI drone in drones) // Loop over drones
        {
            drone.OnPlayerDetected += HandlePlayerDetected; // Subscribe to detection event
        }
    }

    void Update() // Runs every frame
    {
        if (Input.GetButtonDown("Cancel")) // Check pause input
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

    public void statePause() // Pause game logic
    {
        isPaused = true; // Set pause state
        Time.timeScale = 0; // Freeze time
        Cursor.visible = true; // Show cursor
        Cursor.lockState = CursorLockMode.Confined; // Confine cursor

        playerScript.enabled = false; // Disable player controls
    }

    public void stateUnpause() // Unpause game logic
    {
        isPaused = false; // Clear pause state
        Time.timeScale = timeScaleOriginal; // Restore time scale
        Cursor.visible = false; // Hide cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor

        playerScript.enabled = true; // Enable player controls

        if (menuActive != null) // Check active menu
        {
            menuActive.SetActive(false); // Hide menu
            menuActive = null; // Clear active menu
        }
    }

    public void updateGameGoal(int amount) // Update goal logic
    {
        goalCount += amount; // Adjust goal count
        activeThreatsText.text = goalCount.ToString("F0"); // Update UI text

        if (goalCount <= 0) // Check if goal met
        {
            statePause(); // Pause game
            menuActive = menuWin; // Set win menu
            menuActive.SetActive(true); // Show menu
        }
    }

    public void youLose() // Handle lose state
    {
        statePause(); // Pause game
        menuActive = menuLose; // Set lose menu
        menuActive.SetActive(true); // Show menu
    }

    public void HandlePlayerDetected(Vector3 dronePosition) // Drone detects player
    {
        Debug.Log($"Detected at {dronePosition}"); // Log detection
        youLose(); // Trigger lose state
    }

    public void PlayerShoots() // Notify player shot
    {
        Debug.Log("Player shot"); // Log action
        updateGameGoal(-1); // Example goal adjustment
    }

    public void PlayerDied() // Notify player death
    {
        Debug.Log("Player died"); // Log death
        youLose(); // Trigger lose state
    }
}