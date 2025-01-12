using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private RoomManager roomManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rooom manager component from the parent
        roomManager = GetComponentInParent<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is the trigger
        if (other.CompareTag("Player"))
        {
            roomManager.ActivateNextRoom();
        }
    }
}
