using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class Node : MonoBehaviour
{
    [SerializeField] string interactionKey = "t"; // Key to collect the node

    bool isPlayerNearby; // Tracks if the player is in range

    void Update()
    {
        // Check if the player is nearby and presses the interaction key
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            CollectNode(); // Collect the node
        }
    }

    void CollectNode()
    {
        // Notify the GameManager that this node has been collected
        GameManager.instance.NodeCollected();

        // Destroy the node or disable it (based on your design)
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the node's trigger range
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is in range
            GameManager.instance.ShowNodeInteractionUI(); // Notify GameManager to show the UI
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the node's trigger range
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is out of range
            GameManager.instance.HideNodeInteractionUI(); // Notify GameManager to hide the UI
        }
    }
}