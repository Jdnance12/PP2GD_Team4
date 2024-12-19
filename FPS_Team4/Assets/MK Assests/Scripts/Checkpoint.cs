using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool checkpointActivated = false; // Prevents reactivation for this checkpoint
    private static bool instructionsShown = false; // Static ensures it persists across checkpoints

    private GameObject checkpointInstructions; // Reference to the CheckpointInstructions UI
    private GameManager gameManager; // Reference to the GameManager instance

    private void Start()
    {
        // Find GameManager instance
        gameManager = GameManager.instance;

        // Locate the CheckpointInstructions UI
        Transform uiTransform = GameObject.Find("UI")?.transform;
        if (uiTransform != null)
        {
            checkpointInstructions = uiTransform.Find("Player Instructions/CheckpointInstructions")?.gameObject;
        }

        if (checkpointInstructions == null)
        {
            Debug.LogError("CheckpointInstructions UI not found. Ensure it is under UI/Player Instructions in the hierarchy.");
        }
        else
        {
            checkpointInstructions.SetActive(false); // Ensure it's initially off
            Debug.Log("CheckpointInstructions found and initialized.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player enters and checkpoint hasn't been activated
        if (other.CompareTag("Player") && !checkpointActivated)
        {
            checkpointActivated = true; // Prevent reactivation for this checkpoint

            // Save checkpoint position and player stats
            Vector3 savePosition = transform.position;
            savePosition.y += 1f; // Store the position 1 unit above

            PlayerPrefs.SetFloat("Checkpoint_X", savePosition.x);
            PlayerPrefs.SetFloat("Checkpoint_Y", savePosition.y);
            PlayerPrefs.SetFloat("Checkpoint_Z", savePosition.z);

            // Save player stats
            GameManager.instance?.SaveGame(other.transform);

            // Update checkpoint button visibility
            buttonFunctions buttonScript = FindObjectOfType<buttonFunctions>();
            // if (buttonScript != null)
            // {
            //     buttonScript.UpdateCheckpointButtonState();
            // }

            // Show checkpoint UI feedback
            GameManager.instance?.ShowCheckpointUI();
            Debug.Log("Checkpoint activated, position saved.");

            // Display instructions UI only once per game session
            if (!instructionsShown && checkpointInstructions != null)
            {
                instructionsShown = true; // Set the static flag to prevent further triggers
                ShowInstructions();
            }
        }
    }

    private void ShowInstructions()
    {
        Debug.Log("Displaying Checkpoint Instructions UI");

        // Pause the game using GameManager
        gameManager.statePause();

        // Show the instructions UI
        checkpointInstructions.SetActive(true);

        // Connect the Resume button function dynamically
        Transform resumeButton = checkpointInstructions.transform.Find("Resume");
        if (resumeButton != null)
        {
            UnityEngine.UI.Button button = resumeButton.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    HideInstructions();
                });
            }
        }
    }

    private void HideInstructions()
    {
        Debug.Log("Hiding Checkpoint Instructions UI");
        checkpointInstructions.SetActive(false);
        gameManager.stateUnpause(); // Unpause the game
    }
}
