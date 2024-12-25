using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap_PickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int randomScrapAmount = Random.Range(2, 10);
            gameManager.instance.AddScrapCount(randomScrapAmount);
            Destroy(gameObject);
        }
    }
}
