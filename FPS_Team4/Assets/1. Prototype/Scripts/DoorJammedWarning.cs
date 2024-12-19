using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoorJammedWarning : MonoBehaviour
{
    private bool dialogueTriggered = false; // Prevent retriggering the dialogue

    void OnTriggerEnter(Collider other)
    {
        if (dialogueTriggered || !other.CompareTag("Player")) return;

        dialogueTriggered = true;

        // Pause and trigger the Dialogue Screen
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        // Update the UI for the warning message
        UpdateDialogueScreen(
            "!Attention!",
            "The door is <b>jammed</b>. Every part of the factory has a tool set available. Locate a <b>BLUE toolbox</b> to retrieve the tools needed to unjam the door.",
            new Color(1f, 0.85f, 0.2f, 1f) // A yellowish background color for the warning
        );

        Debug.Log("Door Jammed Warning triggered.");
        Destroy(gameObject); // Destroy the collider so it only triggers once
    }

    private void UpdateDialogueScreen(string header, string content, Color backgroundColor)
    {
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        TMP_Text menuText = dialogueScreen.Find("Menu Text")?.GetComponent<TMP_Text>();
        if (menuText != null)
        {
            menuText.text = header;
            Debug.Log("Dialogue header updated.");
        }

        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text")?.GetComponent<TMP_Text>();
        if (dialogueText != null)
        {
            dialogueText.text = content;
            Debug.Log("Dialogue content updated.");
        }

        Image background = dialogueScreen.GetComponent<Image>();
        if (background != null)
        {
            background.color = backgroundColor;
            Debug.Log("Dialogue background color updated.");
        }
    }
}
