using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NodeInformation : MonoBehaviour
{
    private bool dialogueTriggered = false; // Ensures the dialogue only appears once

    void OnTriggerStay(Collider other)
    {
        // Trigger dialogue only once and only if the player is the one colliding
        if (dialogueTriggered || !other.CompareTag("Player")) return;

        dialogueTriggered = true;

        // Pause the game and display the dialogue screen
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        // Update the dialogue screen with the node explanation
        UpdateDialogueScreen(
            "!Node Collection!",
            "Nodes are required to restore the system. Collect all nodes scattered across the areas, <b>BE ON THE LOOKOUT!\n\nOnce collected, return to the Control Room to restart the system.",
            new Color(0.2f, 0.6f, 0.9f, 1f) // Light blue background
        );
    }

    private void UpdateDialogueScreen(string header, string content, Color backgroundColor)
    {
        // Reference the dialogue screen from the GameManager
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        // Update the top header text
        TMP_Text menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
        if (menuText != null) 
        {
            menuText.text = header;
            Debug.Log("Menu Text updated successfully.");
        }
        else
        {
            Debug.LogError("Menu Text not found in DialogueScreen!");
        }

        // Update the body text
        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
        if (dialogueText != null) 
        {
            dialogueText.text = content;
            Debug.Log("Dialogue Text updated successfully.");
        }
        else
        {
            Debug.LogError("Dialogue Text not found in DialogueScreen!");
        }

        // Update the background color
        Image background = dialogueScreen.GetComponent<Image>();
        if (background != null)
        {
            background.color = backgroundColor;
            Debug.Log("Dialogue background color updated successfully.");
        }
        else
        {
            Debug.LogError("Background Image not found in DialogueScreen!");
        }
    }
}
