using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts_PickUp : MonoBehaviour
{
    public GameObject gameManager;
    public UpgradeManager upgradeManager;

    public float fallSpeed;
    public float raycastDist;

    private bool isFalling;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager");
        upgradeManager = gameManager.GetComponent<UpgradeManager>();

        isFalling = true;
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * raycastDist, Color.red);

        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, raycastDist)) 
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    isFalling = false;
                }
            }
        }
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
