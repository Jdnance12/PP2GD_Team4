using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosTrigger : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameManager.progressScript.enemyInPlace = true;
            gameManager.progressScript.enemyStartRoom.GetComponent<SphereCollider>().enabled = true;
            gameManager.progressScript.enemyStartRoom.GetComponent<EnemyBasic>().enabled = true;
            Destroy(gameObject);
        }
    }
}
