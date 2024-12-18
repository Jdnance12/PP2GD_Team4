using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoubleJumpActivator : MonoBehaviour
{

    private playerController playerCtrlr;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image background;

    public bool doubleJump;

    private void Start()
    {
        playerCtrlr = GameManager.instance.playerScript;
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            playerCtrlr.canDoubleJump = true;

            if (playerCtrlr.canDoubleJump == true)
            {
                GameManager.instance.statePause();
                GameManager.instance.DialogueScreen();

                titleText.text = "Double Jump";
                bodyText.text = "You can now double jump by pressing the spacebar a second time while in the air";

                Destroy(gameObject);
            }
        }
    }
}