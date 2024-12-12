using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doubleJump : MonoBehaviour
{
    [SerializeField] string interactionKey = "t"; // Key to collect test object
    [SerializeField] float interactionRange = 2.0f; // Range of interaction radius

    bool isPlayerNearby; // Tracks if the player is in range

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNearby)
            // Check if the player is nearby and presses the interaction key
            if (isPlayerNearby && Input.GetKeyDown(interactionKey))
            {
                GetDoubleJump(); // Obtains double jump skill
            }
    }

    void GetDoubleJump()
    {
        GameManager.instance.GetDoubleJump();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the object trigger range
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player leaves the object trigger range
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is out of range
        }
    }
}
