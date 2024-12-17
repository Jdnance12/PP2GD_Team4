using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InformationPart1 : MonoBehaviour
{
    private bool dialogueTriggered = false;

    void OnTriggerStay(Collider other)
    {
        if (dialogueTriggered || !other.CompareTag("Player")) return;

        dialogueTriggered = true;

        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        UpdateDialogueScreen(
            "!Information Part 1!",
            "Use 'W' to move forward, 'A' to move left, 'S' to move backward, and 'D' to move right.\n\nMove the mouse to look around.",
            new Color(0.5f, 0.8f, 1f, 1f)
        );

        Destroy(gameObject);
    }

    private void UpdateDialogueScreen(string header, string content, Color backgroundColor)
    {
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        TMP_Text menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
        if (menuText != null) menuText.text = header;

        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
        if (dialogueText != null) dialogueText.text = content;

        Image background = dialogueScreen.GetComponent<Image>();
        if (background != null) background.color = backgroundColor;
    }
}
