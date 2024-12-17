using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoubleJumpActivator : MonoBehaviour
{
    private bool dialogueTriggered = false; // Flag to ensure dialogue triggers only once

    private void OnTriggerEnter(Collider other)
    {
        // Check if this has already been triggered
        if (dialogueTriggered) return;

        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            dialogueTriggered = true; // Set flag to prevent re-trigger

            // Activate Double Jump on the Player
            playerController playerCtrlr = other.GetComponent<playerController>();
            if (playerCtrlr != null)
            {
                playerCtrlr.canDoubleJump = true; // Enable double jump
                Debug.Log("Double Jump activated for the player.");
            }

            // Trigger the Dialogue Screen
            DisplayDoubleJumpDialogue();

            // Destroy this object to ensure it cannot be triggered again
            Destroy(gameObject);
        }
    }

    private void DisplayDoubleJumpDialogue()
    {
        // Check for the GameManager instance
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is NULL. Dialogue cannot be triggered.");
            return;
        }

        // Pause the game and show the dialogue screen
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        // Find and update the DialogueScreen UI elements
        GameObject dialogueScreen = GameManager.instance.dialogueScreen;

        if (dialogueScreen != null)
        {
            // Update Header Text 
            TMP_Text menuText = dialogueScreen.transform.Find("Menu Text")?.GetComponent<TMP_Text>();
            if (menuText != null)
            {
                menuText.text = "!Upgrade!";
                Debug.Log("Menu Text updated to '!Upgrade!'.");
            }
            else
            {
                Debug.LogError("Menu Text not found in DialogueScreen.");
            }

            // Update Body Text 
            TMP_Text dialogueText = dialogueScreen.transform.Find("Text Area/Dialogue Text")?.GetComponent<TMP_Text>();
            if (dialogueText != null)
            {
                dialogueText.text = "You've unlocked Double Jump! Press [SPACEBAR] again after jumping once to perform a second jump while in the air.";
                Debug.Log("Dialogue Text updated with Double Jump message.");
            }
            else
            {
                Debug.LogError("Dialogue Text not found in DialogueScreen.");
            }

            // Update Background Color
            Image background = dialogueScreen.GetComponent<Image>();
            if (background != null)
            {
                background.color = Color.green;
                Debug.Log("Dialogue background updated to green.");
            }
            else
            {
                Debug.LogError("Background Image not found in DialogueScreen.");
            }
        }
        else
        {
            Debug.LogError("DialogueScreen GameObject not found in GameManager.");
        }
    }
}