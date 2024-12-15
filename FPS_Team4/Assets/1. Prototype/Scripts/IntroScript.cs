using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScript : MonoBehaviour
{
    public GameObject player;
    public GameObject firstEnemy;
    private SphereCollider enemySphereCollider;
    public CharacterController playerChrController;
    public TextMeshProUGUI uiText;

    public int damageAmmount;

    bool playerIsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        playerChrController = player.GetComponent<CharacterController>();
        enemySphereCollider = firstEnemy.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        playerIsGrounded = playerChrController.isGrounded;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(playerIsGrounded)
            {
                GameManager.instance.statePause();
                GameManager.instance.DialogueScreen();
                GameManager.instance.playerScript.takeDamage(damageAmmount);
                uiText.text = "You've been damaged and need to find a heal station. But you have more pressing issues at the moment. Pick up the weapon from the destroyed bot and kill your attacker!";
                Destroy(gameObject);
            }          
        }
    }
};
