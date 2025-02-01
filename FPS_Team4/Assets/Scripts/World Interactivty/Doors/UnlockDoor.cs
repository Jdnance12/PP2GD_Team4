using System.Collections;
using System.Collections.Generic;
using TMPro; // MK - ADDED: Import for dynamic text updates
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    [SerializeField] private DoorInteractions doorToUnlock; // MK - Changed: Reference to door script
    [SerializeField] private GameObject doorLockedCanvas; // MK - ADDED: UI Canvas for locked door
    [SerializeField] private TMP_Text doorLockedMessage; // MK - ADDED: UI text to display dynamic message

    private bool playerInRange = false; 
    private GameManager gm; // MK - ADDED: Reference to GameManager
    private int requiredNodes = 3; // MK - ADDED: Nodes required to unlock door
    private bool wasCanvasActiveBeforePause = false; // MK - ADDED: Tracks if message was visible before pause

    void Start()
    {
        gm = GameManager.instance; // MK - ADDED: Get GameManager instance
        if (doorLockedCanvas != null) 
        {
            doorLockedCanvas.SetActive(false); // MK - ADDED: Ensure UI is disabled at start
        }
    }

    // Update is called once per frame
    void Update()
    {
        // MK - ADDED START: Hide message when game is paused
        if (gm.isPaused) 
        {
            if (doorLockedCanvas != null && doorLockedCanvas.activeSelf) 
            {
                wasCanvasActiveBeforePause = true; // Remember that it was active
                doorLockedCanvas.SetActive(false); // Temporarily hide UI
            }
            return; // Stop further checks while paused
        }
        else if (wasCanvasActiveBeforePause) 
        {
            doorLockedCanvas.SetActive(true); // Restore message when unpaused
            wasCanvasActiveBeforePause = false; // Reset flag
        }
        // MK - ADDED END

        if (playerInRange) // MK - ADDED: Only check input if player is in range
        {
            if (gm.nodeCount >= requiredNodes) // MK - ADDED: Checks if player has enough nodes
            {
                if (Input.GetButtonDown("Interact")) // MK - CHANGED: Ensures input is detected on press
                {
                    doorToUnlock.UnlockDoor(); // Unlock door
                    GetComponent<Collider>().enabled = false; // Disable trigger

                    if (doorLockedCanvas != null && doorLockedMessage != null) 
                    {
                        doorLockedCanvas.SetActive(true); // MK - ADDED: Ensure message appears
                        doorLockedMessage.text = "Door Unlocked!"; // MK - ADDED: Update text dynamically
                        StartCoroutine(HideMessageAfterDelay(2f)); // MK - ADDED: Hide message after 2 seconds
                    }
                }
            }
            else
            {
                if (doorLockedCanvas != null && doorLockedMessage != null) 
                {
                    doorLockedCanvas.SetActive(true); // MK - ADDED: Show message when player lacks nodes
                    doorLockedMessage.text = "Door Locked: Collect " + (requiredNodes - gm.nodeCount) + " more nodes."; // MK - ADDED: Update text dynamically
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // MK - CHANGED: Player is now in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // MK - CHANGED: Player left the range
            if (doorLockedCanvas != null) 
            {
                doorLockedCanvas.SetActive(false); // MK - ADDED: Hide message when player leaves
            }
        }
    }

    IEnumerator HideMessageAfterDelay(float delay) // MK - ADDED: Hide UI after delay
    {
        yield return new WaitForSeconds(delay); // Wait before hiding
        if (doorLockedCanvas != null) 
        {
            doorLockedCanvas.SetActive(false); // Hide UI
        }
    }
}