using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AfterHealStation : MonoBehaviour
{
    [SerializeField] GameObject rampUp;  // Reference to the ramp up
    [SerializeField] GameObject rampDown; // Reference to the ramp down
    [SerializeField] GameObject enemy;    // Reference to the enemy

    private bool playerInRange = false;   // Tracks if player is in range
    private bool dialogueTriggered = false; // Ensures dialogue only triggers once

    void Start()
    {
        Debug.Log("AfterHealStation initialized.");
    }

    void Update()
    {
        // Check for player interaction to heal
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            HealPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || dialogueTriggered) return;

        dialogueTriggered = true; // Prevent duplicate triggers

        Debug.Log("Player entered heal station trigger. Displaying dialogue screen");

        // Pause the game and display the dialogue screen
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        // Update the dialogue screen with the information
        UpdateDialogueScreen(
            "!Information!",
            "Stand on the platform of the Healing Station to heal.",
            new Color(0.1f, 0.5f, 0.8f, 1f) // Blue background
        );
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // Allow healing when player is in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Disable healing when player exits
        }
    }

    void HealPlayer()
    {
        Debug.Log("Healing player and activating ramps/enemy");

        // Heal player to full HP
        playerController playerScript = GameManager.instance.player.GetComponent<playerController>();
        if (playerScript != null)
        {
            playerScript.HealToMax(); // Call the function to heal
            Debug.Log("Player healed to max health.");
        }
        else
        {
            Debug.LogError("PlayerController not found on the player object.");
        }

        // Toggle ramps and activate enemy
        rampUp.SetActive(false);
        Debug.Log("RampUp deactivated.");
        rampDown.SetActive(true);
        Debug.Log("RampDown activated.");
        enemy.SetActive(true);
        Debug.Log("Enemy activated.");

        // Destroy the heal station object after interaction
        Debug.Log("Destroying AfterHealStation object");
        Destroy(gameObject);
    }

    private void UpdateDialogueScreen(string header, string content, Color backgroundColor)
    {
        Debug.Log("Updating dialogue screen content");

        // Get the Dialogue Screen components
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        TMP_Text menuText = dialogueScreen.Find("Menu Text")?.GetComponent<TMP_Text>();
        if (menuText != null)
        {
            menuText.text = header;
            Debug.Log("Menu Text updated.");
        }
        else
        {
            Debug.LogError("Menu Text not found in DialogueScreen!");
        }

        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text")?.GetComponent<TMP_Text>();
        if (dialogueText != null)
        {
            dialogueText.text = content;
            Debug.Log("Dialogue Text updated.");
        }
        else
        {
            Debug.LogError("Dialogue Text not found in DialogueScreen!");
        }

        Image background = dialogueScreen.GetComponent<Image>();
        if (background != null)
        {
            background.color = backgroundColor;
            Debug.Log("Background color updated.");
        }
        else
        {
            Debug.LogError("Background Image not found in DialogueScreen!");
        }
    }
}