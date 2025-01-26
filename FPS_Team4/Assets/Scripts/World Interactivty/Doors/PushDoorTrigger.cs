using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushDoorTrigger : MonoBehaviour
{
    public DoorController doorController;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetButton("Interact"))
        {
            doorController.ToggleDoor();
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
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
