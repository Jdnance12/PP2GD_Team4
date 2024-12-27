using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts_PickUp : MonoBehaviour
{

    //public Upgrade_Menu menu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int randomScrapAmount = Random.Range(2, 10);
            gameManager.instance.upgradeMenuScript.AddPartsCount(randomScrapAmount);
            //menu.AddPartsCount(randomScrapAmount);
            Destroy(gameObject);
        }
    }
}
