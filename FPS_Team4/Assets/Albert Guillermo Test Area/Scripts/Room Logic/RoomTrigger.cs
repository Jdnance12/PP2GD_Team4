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
        if (roomManager == null )
        {
            Debug.LogError("RoomManager not found in parent");
            // This should help if we forget to put a RoomManager script on any of the room variations!
        }

        // Get the collider component on this game object
        triggerCollider = GetComponent<Collider>();
        if(triggerCollider == null )
        {
            Debug.LogError("Collider not found on the RoomTrigger game object.");
            // There should always be a collider on the trigger object.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is the trigger
        if (other.CompareTag("Player"))
        {
            if(roomManager != null)
            {
                roomManager.ActivateNextRoom(doorIndex); // pass door index
                Debug.Log("Activated next room for door index: " + doorIndex);
                // Helpful if its difficult to tell which variation is being used.
            }
            

            // Deactivate the trigger so it cannot be triggered again
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
                Debug.Log("Deactivated trigger collider");
                // check if trigger collider is deactivated by mistake.
            }
        }
    }
}
