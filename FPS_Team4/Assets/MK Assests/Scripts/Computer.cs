using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private string interactionKey = "t"; // Key for interaction
    private bool isPlayerNearby = false; // Tracks if the player is in range
    private bool isInteractable = false; // Tracks if the computer can be interacted with

    void Start()
    {
        // Report this computer to the GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.computer = this.gameObject;

            // Ensure UI elements are hidden at the start
            GameManager.instance.HideComputerUI();
            Debug.Log("Computer successfully reported to GameManager.");
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Ensure GameManager exists in the scene.");
        }
    }

    void Update()
    {
        // Handle player interaction with the computer
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateComputerUI(); // Update UI based on game state
                GameManager.instance.TryWinGame(); // Attempt to win the game
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            isPlayerNearby = true; // Mark player as nearby
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateComputerUI(); // Show appropriate UI when player is in range
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player exits the trigger
        {
            isPlayerNearby = false; // Mark player as no longer nearby
            if (GameManager.instance != null)
            {
                GameManager.instance.HideComputerUI(); // Hide UI when player is out of range
            }
        }
    }

    public void EnableInteraction()
    {
        isInteractable = true; // Make the computer interactable
        Debug.Log("Computer is now interactable.");
    }
}
