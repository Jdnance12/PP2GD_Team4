using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurretFactoryWarning : MonoBehaviour
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
            "!Warning: Entering Area 2!",
            "Caution: You are now entering the <b>Turret Factory</b>. \n\nThe automated turrets in this area are <b>malfunctioning</b> and will target anything that is not one of them.\n\nProceed carefully and <b>stay alert!</b>",
            new Color(0.8f, 0f, 0f, 1f) // A deep red background for added urgency
        );

        Debug.Log("Turret Factory Warning triggered.");
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
