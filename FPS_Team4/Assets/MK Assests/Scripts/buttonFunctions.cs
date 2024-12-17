using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; //for TextMeshPro
using System.Collections;

public class buttonFunctions : MonoBehaviour
{
    [Header("General UI References")]
    [SerializeField] private GameObject welcomeScreen; // Welcome screen reference
    [SerializeField] private GameObject reticle;       // Reticle reference
    [SerializeField] private GameObject playerHP;      // Player HP bar reference
    [SerializeField] private GameObject nodesCollectionLabel; // Nodes collection label reference
    [SerializeField] private GameObject checkpointInstructions; // Checkpoint instructions UI
    [SerializeField] private GameObject pauseMenu; // Pause menu reference
    //[SerializeField] private GameObject doorInteractionPrompt; // reference to door info (Doors Prompt)

    [SerializeField] private TMP_Text loadingText;        // Reference to the "Loading..." text
    [SerializeField] private TextMeshProUGUI countdownText; // Countdown UI text
    [SerializeField] private GameObject countdownUI; // Parent GameObject for countdown UI

    private bool instructionsShown = false; // Track if instructions were shown
    private bool canResume = true; // Cooldown flag

    [Header("Checkpoint Buttons")]
    [SerializeField] private GameObject pauseCheckpointButton;    // Pause Menu Checkpoint Button
    [SerializeField] private GameObject loseMenuCheckpointButton; // Lose Menu Checkpoint Button

    void Start()
    {
        // Ensure checkpoint buttons are initially turned off
        if (pauseCheckpointButton != null)
        {
            pauseCheckpointButton.SetActive(false);
        }

        if (loseMenuCheckpointButton != null)
        {
            loseMenuCheckpointButton.SetActive(false);
        }

        // Call UpdateCheckpointButtonState to check for valid save data
        UpdateCheckpointButtonState();

        ////check to make sure things are assigned
        //if(doorInteractionPrompt == null)
        //{
        //    Debug.LogError("door prompt is not assigned.");
        //}
    }


    public void Play()
    {
        // Hide Start Screen and show Checkpoint Instructions if needed
        //if (GameManager.instance.startScreen != null)
        //{
        //    GameManager.instance.startScreen.SetActive(false);
        //}
        //ShowCheckpointInstructionsOnce();
    }
    public void Resume()
    {
        if (!canResume) return; // Prevent immediate interaction
        canResume = false;
        StartCoroutine(ResumeCooldown());

        if (GameManager.instance.menuActive != GameManager.instance.dialogueScreen)
        {
            if (pauseMenu != null) pauseMenu.SetActive(false);
            GameManager.instance.menuActive = null;
        }

        GameManager.instance.stateUnpause();
        Debug.Log("Game resumed. Pause Menu hidden.");
    }


    private IEnumerator ResumeCooldown()
    {
        yield return new WaitForSeconds(0.5f); // Add a 0.5-second cooldown
        canResume = true;
    }


    //public void StartScreenResume()
    //{
    //    if (GameManager.instance.startScreen != null && GameManager.instance.startScreen.activeSelf)
    //    {
    //        // Hide Start Screen and show checkpoint instructions
    //        GameManager.instance.startScreen.SetActive(false);
    //        ShowCheckpointInstructionsOnce();
    //        Debug.Log("Start Screen hidden. Checkpoint Instructions shown.");
    //    }
    //}

    public void CheckpointInstructionsResume()
    {
        if (checkpointInstructions != null && checkpointInstructions.activeSelf)
        {
            // Turn off Checkpoint Instructions
            checkpointInstructions.SetActive(false);
            GameManager.instance.stateUnpause();
            Debug.Log("Checkpoint Instructions turned off. Game unpaused.");
        }
    }

    public void Restart()
    {
        // Reload the current scene and unpause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance?.stateUnpause();
    }

    public void Quit()
    {
        // Check if there is save data
        if (PlayerPrefs.HasKey("Player_X") || PlayerPrefs.HasKey("Player_Y") || PlayerPrefs.HasKey("Player_Z"))
        {
            Debug.Log("Save data found. Clearing all save data on Quit.");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save(); // Ensure the data is immediately saved
        }
        else
        {
            Debug.Log("No save data found. Proceeding to Quit.");
        }

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
    #else
        Application.Quit(); // Quit the application
    #endif
    }

    public void OnLoadCheckpointButton()
    {
        if (GameManager.instance != null)
        {
            Debug.Log("Loading Checkpoint...");
            
            // Hide Pause Menu and load the game
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }

            if (GameManager.instance != null)
            {
                GameManager.instance.LoadGame();
            }
            else
            {
                Debug.LogError("GameManager instance is null. Cannot load checkpoint.");
            }

            // Start countdown coroutine
            StartCoroutine(ShowCountdown());
        }
    }

    private IEnumerator ShowCountdown()
    {
        // Ensure the countdown UI is visible
        countdownUI.SetActive(true);
        
        // Display "Loading..." text 
        if (loadingText != null)
        {
            loadingText.text = "Loading...";
        }

        // Countdown logic
        int countdown = 3;

        while (countdown > 0)
        {
            // Update countdown numbers
            countdownText.text = countdown.ToString();
            
            yield return new WaitForSecondsRealtime(1f); // Wait for 1 real-time second
            countdown--;
        }

        // Hide the countdown UI after the countdown finishes
        countdownUI.SetActive(false);

        // Re-enable the pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }

        canResume = true; // Allow "Resume" button interaction
    }

    public void OnLoadGameFromDeathMenu()
    {
        Debug.Log("Loading from Death Menu...");

        // Turn off "You Lose" menu immediately
        if (GameManager.instance.menuLose != null)
        {
            GameManager.instance.menuLose.SetActive(false);
            Debug.Log("You Lose menu turned off.");
        }

        // Move the player to the checkpoint immediately
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadGame();
        }
        else
        {
            Debug.LogError("GameManager instance is null. Cannot load checkpoint.");
        }
        // Hide Pause Menu during countdown
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        // Pause the entire game (stop enemies, etc.)
        Time.timeScale = 0;

        // Start the countdown coroutine (using real-time)
        StartCoroutine(LoadCheckpointWithCountdown());
    }

    private IEnumerator LoadCheckpointWithCountdown()
    {
        Debug.Log("Starting Loading Countdown...");

        // Show Loading UI
        countdownUI.SetActive(true);

        // Display "Loading..." text
        if (loadingText != null)
        {
            loadingText.text = "Loading...";
        }

        // Countdown logic: use real-time unaffected by Time.timeScale
        int countdown = 3;
        while (countdown > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = countdown.ToString();
            }

            yield return new WaitForSecondsRealtime(1f); // Wait 1 real-time second
            countdown--;
        }

        // Hide the Loading UI
        countdownUI.SetActive(false);

        // Resume the game
        GameManager.instance.statePause(); // Game remains paused for the player to resume manually

        // Show the Pause Menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }

        Debug.Log("Loading complete. Pause Menu shown.");
    }

    public void UpdateCheckpointButtonState()
    {
        bool hasValidSaveData = PlayerPrefs.HasKey("Player_X") 
                                && PlayerPrefs.HasKey("Player_Y") 
                                && PlayerPrefs.HasKey("Player_Z");

        // Additional validation Ensure saved position is reasonable
        if (hasValidSaveData)
        {
            float x = PlayerPrefs.GetFloat("Player_X");
            float y = PlayerPrefs.GetFloat("Player_Y");
            float z = PlayerPrefs.GetFloat("Player_Z");

            hasValidSaveData = !(x == 0 && y == 0 && z == 0); // Reject invalid positions
        }

        Debug.Log($"Checkpoint Button Visibility: {hasValidSaveData}");

        if (pauseCheckpointButton != null)
        {
            pauseCheckpointButton.SetActive(hasValidSaveData);
            Debug.Log($"PauseCheckpointButton active: {pauseCheckpointButton.activeSelf}");
        }

        if (loseMenuCheckpointButton != null)
        {
            loseMenuCheckpointButton.SetActive(hasValidSaveData);
            Debug.Log($"LoseMenuCheckpointButton active: {loseMenuCheckpointButton.activeSelf}");
        }
    }


    private void ShowCheckpointInstructionsOnce()
    {
        if (!instructionsShown && checkpointInstructions != null)
        {
            checkpointInstructions.SetActive(true);
            instructionsShown = true;
        }
    }

    public void DisableAllCheckpointButtons()
    {
        if (pauseCheckpointButton != null)
        {
            pauseCheckpointButton.SetActive(false);
            Debug.Log("PauseCheckpointButton forced to be disabled.");
        }

        if (loseMenuCheckpointButton != null)
        {
            loseMenuCheckpointButton.SetActive(false);
            Debug.Log("LoseMenuCheckpointButton forced to be disabled.");
        }
    }

    //public void OnDoorInteractionButtonClicked()
    //{
    //    // Assuming doorInteractionPromptObject already references the correct GameObject
    //    DoorInteractionPrompt doorInteractionPromptComponent = doorInteractionPrompt.GetComponent<DoorInteractionPrompt>();

    //    if (doorInteractionPromptComponent != null)
    //    {
    //        doorInteractionPromptComponent.OnButtonClicked();
    //    }
    //    else
    //    {
    //        Debug.LogError("DoorInteractionPrompt component not found.");
    //    }
    //}

}