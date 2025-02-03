using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    [SerializeField] private Door doorToUnlock;
    private bool playerInRange = false;

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButton("Interact"))
        {
            // unlock associated door
            doorToUnlock.UnlockDoor();
            // deactivate trigger after
            GetComponent<Collider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
