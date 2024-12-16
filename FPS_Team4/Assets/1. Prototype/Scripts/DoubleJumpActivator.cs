using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoubleJumpActivator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            playerController playerCtrlr = other.GetComponent<playerController>(); // Enable double jump
            if (playerCtrlr != null)
            {
                playerCtrlr.canDoubleJump = true;
                Debug.Log("Double Jump activated for the player.");
            }

            // Trigger dialogue screen in GameManager
            GameManager.instance.DialogueScreen();

            // Find and update the DialogueScreen UI elements
            GameObject dialogueScreen = GameManager.instance.dialogueScreen;

            if (dialogueScreen != null)
            {
                // Change the top text
                Transform menuText = dialogueScreen.transform.Find("Menu Text");
                if (menuText != null)
                {
                    TMP_Text menuTextComponent = menuText.GetComponent<TMP_Text>();
                    if (menuTextComponent != null)
                    {
                        menuTextComponent.text = "!Upgrade!";
                    }
                }

                // Update with double jump text
                Transform textArea = dialogueScreen.transform.Find("Text Area/Dialogue Text");
                if (textArea != null)
                {
                    TMP_Text dialogueTextComponent = textArea.GetComponent<TMP_Text>();
                    if (dialogueTextComponent != null)
                    {
                        dialogueTextComponent.text = "You've unlocked Double Jump! Press [SPACEBAR] again after jumping once to perform a second jump while in the air.";
                    }
                }

                // Change the DialogueScreen background color to green
                Image background = dialogueScreen.GetComponent<Image>();
                if (background != null)
                {
                    background.color = Color.green;
                }
            }

            // Destroy this object to prevent reactivation
            Destroy(gameObject);
        }
    }
}
