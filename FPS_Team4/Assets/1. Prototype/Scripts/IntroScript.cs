using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScript : MonoBehaviour
{
    public GameObject player;
    public GameObject firstEnemy;
    private SphereCollider enemySphereCollider;
    public CharacterController playerChrController;
    private bool dialogueTriggered = false; // Flag to prevent retriggering dialogue

    public int damageAmmount;

    bool playerIsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        playerChrController = player.GetComponent<CharacterController>();

        // Find the first enemy by name
        firstEnemy = GameObject.Find("Enemy_Soldier_FirstEnemy");
        if (firstEnemy != null)
        {
            enemySphereCollider = firstEnemy.GetComponent<SphereCollider>();
            Debug.Log("First Enemy found and SphereCollider assigned.");
        }
        else
        {
            Debug.LogError("First Enemy 'Enemy_Soldier_FirstEnemy' not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerIsGrounded = playerChrController.isGrounded;
    }

    void OnTriggerStay(Collider other)
    {
        if (dialogueTriggered) return; // Exit if dialogue already triggered

        if (other.CompareTag("Player"))
        {
            if (playerIsGrounded)
            {
                dialogueTriggered = true; // Mark dialogue as triggered

                // Pause the game and show the Dialogue Screen
                GameManager.instance.statePause();
                GameManager.instance.DialogueScreen();

                Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

                // Update Menu Text 
                TMP_Text menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
                if (menuText != null)
                {
                    menuText.text = "!Warning!";
                }

                // Update Dialogue Text
                TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
                if (dialogueText != null)
                {
                    dialogueText.text = "You've been damaged and need to find a heal station. But you have more pressing issues at the moment. Pick up the weapon from the destroyed bot and kill your attacker!\n\n<b>Another message coming in.</b>";
                }

                // Set the background color
                Image background = dialogueScreen.GetComponent<Image>();
                if (background != null)
                {
                    background.color = Color.red; // Set background color to red
                }

                GameManager.instance.playerScript.takeDamage(damageAmmount); // Damage the player

                Destroy(gameObject); // Destroy this object

                // Start the chain of dialogue screens
                GameManager.instance.StartCoroutine(ShowSecondMessage());
            }
        }
    }

    // First follow-up message
    private IEnumerator ShowSecondMessage()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 seconds

        // Show second message
        ShowDialogueScreen(
            "!Information Part 1!",
            "Use 'W' to move forward, 'A' to move left, 'S' to move backward, and 'D' to move right.\n\nMove the mouse to look around.\n\n<b>Another message coming in.</b>",
            new Color(0.5f, 0.8f, 1f, 1f) // Light blue color
        );

        // Start coroutine for the third message
        GameManager.instance.StartCoroutine(ShowThirdMessage());
    }

    // Second follow-up message
    private IEnumerator ShowThirdMessage()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 seconds

        // Show second message
        ShowDialogueScreen(
            "!Information Part 2!",
            "Tap the [SPACEBAR] to Jump.\n\nActivate Sprint mode by tapping [LEFT SHIFT], and deactivate it by tapping [LEFT SHIFT] again.\n\n<b>Last message coming in.</b>",
            new Color(0.5f, 0.8f, 1f, 1f) // Light blue color
        );

        // Start coroutine for the third message
        GameManager.instance.StartCoroutine(ShowForthMessage());
    }

    // Third follow-up message
    private IEnumerator ShowForthMessage()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 seconds 

        // Show third message
        ShowDialogueScreen(
            "!Combat Training!",
            "Use the mouse to aim.\n\nPress the [LEFT MOUSE BUTTON] to shoot and attack enemies.\n\n<b>End Messages. Be ready!</b>",
            new Color(0.6f, 0.9f, 0.6f, 1f) // Light green color for the combat tutorial
        );
    }

    // Utility function to display different dialogue screens
    private void ShowDialogueScreen(string menuHeader, string dialogueContent, Color backgroundColor)
    {
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        // Update Menu Text
        TMP_Text menuText = dialogueScreen.Find("Menu Text").GetComponent<TMP_Text>();
        if (menuText != null)
        {
            menuText.text = menuHeader;
        }

        // Update Dialogue Text
        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text").GetComponent<TMP_Text>();
        if (dialogueText != null)
        {
            dialogueText.text = dialogueContent;
        }

        // Set Background Color
        Image background = dialogueScreen.GetComponent<Image>();
        if (background != null)
        {
            background.color = backgroundColor;
        }
    }
}