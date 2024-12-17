using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatTraining : MonoBehaviour
{
    private bool dialogueTriggered = false;

    void OnTriggerStay(Collider other)
    {
        if (dialogueTriggered || !other.CompareTag("Player")) return;

        dialogueTriggered = true;

        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        UpdateDialogueScreen(
            "!Combat Training!",
            "Use the mouse to aim.\n\nPress the [LEFT MOUSE BUTTON] to shoot and attack enemies.\n\n<b>Be ready!</b>",
            new Color(0.6f, 0.9f, 0.6f, 1f)
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