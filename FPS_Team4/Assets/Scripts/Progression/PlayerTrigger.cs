using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    GameManager gameManager;
    public GameObject gunButton;
    public GameObject bladeButton;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.progressScript.firstBossKilled = true;
            gameManager.progressScript.playerCanMove = true;
            gameManager.progressScript.gunButton.SetActive(false);
            gameManager.progressScript.bladeButton.SetActive(true);
            gameManager.playerScript.gunActive = false;
            
        }
    }
}
