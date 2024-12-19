using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrappleHookActivator : MonoBehaviour
{

    private playerController playerCtrlr;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image background;

    // Start is called before the first frame update
    void Start()
    {
        playerCtrlr = GameManager.instance.playerScript;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCtrlr.grappleHookActive = true;

            if (playerCtrlr.grappleHookActive == true)
            {
                GameManager.instance.statePause();
                GameManager.instance.DialogueScreen();

                titleText.text = "Grapple Hook";
                bodyText.text = "Holding V will shoot your grapple hook and you'll move towards it's hit location." +
                    "Unless you are aiming at a Heavy Object, you'll be able to pull it out of your way.";

                Destroy(gameObject);
            }
        }
    }
}
