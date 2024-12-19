using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JammedDoor : MonoBehaviour
{
    private bool playerInTrigger = false; // Tracks if the player is inside the trigger
    private TMP_Text menuText;
    private TMP_Text dialogueText;
    private Image background;

    void Start()
    {
        // Pre-fetch dialogue screen components to avoid repeated searches
        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
        dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
        background = dialogueScreen.GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerCtrlr = other.GetComponent<playerController>();
            if (playerCtrlr != null && playerCtrlr.canUnjamDoors)
            {
                playerInTrigger = true;
                ShowUnjamDoorPrompt();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            HideDialogueScreen();
        }
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.T))
        {
            UnjamDoor();
        }
    }

    private void ShowUnjamDoorPrompt()
    {
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        if (menuText != null)
            menuText.text = "!Door Jammed!";

        if (dialogueText != null)
            dialogueText.text = "Press <b>['t']</b> to unjam the door and proceed.";

        if (background != null)
            background.color = new Color(0.9f, 0.7f, 0.2f, 1f); // Yellowish color

        Debug.Log("Unjam Door Prompt displayed.");
    }

    private void HideDialogueScreen()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.stateUnpause();
            GameManager.instance.dialogueScreen.SetActive(false);
        }
        Debug.Log("Unjam Door Prompt hidden.");
    }

    private void UnjamDoor()
    {
        Debug.Log("Door unjammed. Removing from scene.");
        HideDialogueScreen();
        Destroy(gameObject); // Deletes the door object from the scene
    }
}