using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BeginningScript : MonoBehaviour
{

    public GameObject player;
    public CharacterController playerChrController;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image background;
    

    public int damageAmmount;

    bool playerIsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        playerChrController = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerIsGrounded = playerChrController.isGrounded;

    }

    private void OnTriggerStay(Collider other)
    {
        if (playerIsGrounded)
        {
            GameManager.instance.statePause();
            GameManager.instance.DialogueScreen();

            background.color = Color.red;
            titleText.text = "!Warning!";
            bodyText.text = "You've been damaged and need to find a heal station. " +
                "But you have more pressing issues at the moment. " +
                "Move (W,S,A,D) to the weapon near by! An enemy draws near!";

            Destroy(gameObject);
        }
    }
}
