using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerV2 : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            gameManager.progressScript.playerLanded = true;
            Destroy(gameObject);
        }
    }
}
