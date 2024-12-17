using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FirstWarning : MonoBehaviour
{
    public GameObject player;
    public int damageAmount = 2; // Damage inflicted on player
    private bool dialogueTriggered = false; // Prevent retriggering

    void Start()
    {
        player = GameManager.instance.player;
    }

    void OnTriggerStay(Collider other)
    {
        if (dialogueTriggered || !other.CompareTag("Player")) return;

        dialogueTriggered = true;

        // Pause and trigger Dialogue Screen
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        // Update UI for warning
        UpdateDialogueScreen(
            "!Warning!",
            "You've been damaged and need to find a heal station. But you have more pressing issues at the moment. Pick up the weapon from the destroyed bot and kill your attacker!",
            Color.red
        );

        // Damage player and destroy collider
        GameManager.instance.playerScript.takeDamage(damageAmount);
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
