using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolboxPickup : MonoBehaviour
{
    private TMP_Text menuText;
    private TMP_Text dialogueText;
    private Image background;

    private void Start()
    {
        // Pre-fetch dialogue screen components to avoid repeated searches
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
        dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
        background = dialogueScreen.GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check collider to the player
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Collider does not belong to the player.");
            return;
        }

        Debug.Log("Player detected. Attempting to give ability");

        // Notify the GameManager that the toolbox has been picked up
        if (GameManager.instance != null)
        {
            GameManager.instance.ToolboxPickedUp();
        }
        else
        {
            Debug.LogError("GameManager instance is NULL.");
        }

        // Enable the player ability to unjam doors
        playerController playerCtrlr = other.GetComponent<playerController>();
        if (playerCtrlr != null)
        {
            playerCtrlr.canUnjamDoors = true; // Grant the player the ability
            Debug.Log("Toolbox picked up. Player can now unjam doors.");
        }
        else
        {
            Debug.LogError("playerController not found on Player.");
        }

        // Display Upgrade Dialogue Screen
        ShowUpgradeDialogue();

        // Destroy the toolbox object to mark it as picked up
        Destroy(gameObject);
        Debug.Log("Toolbox destroyed.");
    }

    private void ShowUpgradeDialogue()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.statePause();
            GameManager.instance.DialogueScreen();

            if (menuText != null)
                menuText.text = "!Upgrade!";

            if (dialogueText != null)
                dialogueText.text = "You picked up the <b>Tool Set</b>!\nYou can now <b>unjam doors</b> blocking your path. Press [T] near a jammed door to unjam it.";

            if (background != null)
                background.color = new Color(0.2f, 0.6f, 0.2f, 1f); // Green background for upgrade
        }
        else
        {
            Debug.LogError("GameManager instance is NULL. Cannot show dialogue screen.");
        }
    }
}
