using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] string interactionKey = "t"; // Key to collect the node
    [SerializeField] float interactionRange = 2.0f; // Range within which the player can interact with the node

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the node's trigger range
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is out of range
        }
    }
}