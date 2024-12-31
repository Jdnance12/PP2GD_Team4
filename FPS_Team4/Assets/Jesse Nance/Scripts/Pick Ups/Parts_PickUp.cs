using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts_PickUp : MonoBehaviour
{
    public GameObject gameManager;
    public UpgradeManager upgradeManager;
    //public Upgrade_Menu menu;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager");
        upgradeManager = gameManager.GetComponent<UpgradeManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int randomScrapAmount = Random.Range(2, 10);
            upgradeManager.AddPartsCount(randomScrapAmount);

            Destroy(gameObject);
        }
    }
}
