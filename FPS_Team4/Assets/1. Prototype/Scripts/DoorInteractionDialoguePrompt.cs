using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoorInteractionDialoguePrompt : MonoBehaviour
{
    //[Header("Dialogue Settings")]
    //[SerializeField] private string headerText = "**SYSTEM ALERT!**";  // Header for the dialogue
    //[SerializeField] private string contentText = "You can open doors by approaching them!"; // Content for the dialogue
    //[SerializeField] private Color backgroundColor = new Color(0.5f, 0.8f, 1f, 1f); // Background color

    //[Header("Trigger Settings")]
    //[SerializeField] private bool showOnce = true; // Only show the popup once
    //private bool wasTriggered = false;            // Track if the dialogue has already shown

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (wasTriggered || !other.CompareTag("Player")) return;

    //    if (GameManager.instance != null) // Ensure GameManager exists
    //    {
    //        wasTriggered = true; // Prevent re-triggering
    //        Debug.Log("Trigger entered. Showing dialogue screen.");

    //        // Pause the game and activate the dialogue screen
    //        GameManager.instance.statePause();
    //        GameManager.instance.DialogueScreen();

    //        UpdateDialogueScreen(headerText, contentText, backgroundColor);
    //    }
    //    else
    //    {
    //        Debug.LogError("GameManager instance is null. Cannot show dialogue screen.");
    //    }
    //}

    //private void UpdateDialogueScreen(string header, string content, Color bgColor)
    //{
    //    Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

    //    // Set the header text
    //    TMP_Text menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
    //    if (menuText != null)
    //    {
    //        menuText.text = header;
    //        Debug.Log("Header text set: " + header);
    //    }
    //    else
    //    {
    //        Debug.LogError("Menu Text not found in DialogueScreen.");
    //    }

    //    // Set the content text
    //    TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
    //    if (dialogueText != null)
    //    {
    //        dialogueText.text = content;
    //        Debug.Log("Content text set: " + content);
    //    }
    //    else
    //    {
    //        Debug.LogError("Dialogue Text not found in DialogueScreen.");
    //    }

    //    // Set the background color
    //    Image background = dialogueScreen.GetComponent<Image>();
    //    if (background != null)
    //    {
    //        background.color = bgColor;
    //        Debug.Log("Background color set.");
    //    }
    //    else
    //    {
    //        Debug.LogError("Background Image not found in DialogueScreen.");
    //    }
    //}

}
