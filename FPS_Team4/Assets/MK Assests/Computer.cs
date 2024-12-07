using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private string interactionKey = "t"; // Key for interaction
    private bool isPlayerNearby = false; // Tracks if the player is in range
    private bool isInteractable = false; // Tracks if the computer can be interacted with

    void Update()
    {
        // Check if the player presses the interaction key while nearby
        if (isPlayerNearby && isInteractable && Input.GetKeyDown(interactionKey))
        {
            GameManager.instance.TryWinGame(); // Notify GameManager to attempt winning the game
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the colliding object is the player
        {
            isPlayerNearby = true; // Player is in range
            ShowInteractionPrompt(true); // Show interaction UI prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the exiting object is the player
        {
            isPlayerNearby = false; // Player is no longer in range
            ShowInteractionPrompt(false); // Hide interaction UI prompt
        }
    }

    public void EnableInteraction()
    {
        isInteractable = true; // Make the computer interactable
        Debug.Log("Computer is now interactable."); // Debug message
    }

    private void ShowInteractionPrompt(bool show)
    {
        // Logic for showing or hiding the interaction prompt
        if (show)
        {
            Debug.Log($"Press {interactionKey} to interact with the computer."); // Replace with actual UI logic
        }
        else
        {
            Debug.Log("Interaction prompt hidden."); // Replace with actual UI logic
        }
    }
}
