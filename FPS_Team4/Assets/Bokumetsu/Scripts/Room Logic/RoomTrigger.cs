using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private RoomManager roomManager;

    private Collider triggerCollider;

    [SerializeField] private int doorIndex; // identify which door this is for

    // Start is called before the first frame update
    void Start()
    {
        // Get the rooom manager component from the parent
        roomManager = GetComponentInParent<RoomManager>();

        // Get the collider component on this game object
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is the trigger
        if (other.CompareTag("Player"))
        {
            roomManager.ActivateNextRoom(doorIndex); // pass door index

            // Deactivate the trigger so it cannot be triggered again
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }
}
