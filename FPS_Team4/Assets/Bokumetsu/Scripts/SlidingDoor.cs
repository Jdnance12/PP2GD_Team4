using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private Vector3 openPosition;
    private Vector3 closedPosition;

    [SerializeField] private float slideSpeed = 1.0f; // Speed of the sliding door

    private bool isOpening = false;

    private Transform doorTransform; // Reference to the door's transform

    // Start is called before the first frame update
    void Start()
    {
        // Get the door transform (assuming the door is the first child)
        if (transform.childCount > 0)
        {
            // Get the door transform (assuming the door is the first child)
            doorTransform = transform.GetChild(0);

            // Set the initial closed position of the door
            closedPosition = doorTransform.localPosition;
            Debug.Log("Door initialized. Closed position: " + closedPosition);
        }
        else
        {
            Debug.LogError("No child object found for doorTransform. Ensure the door is a child of the parent GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doorTransform == null)
        {
            Debug.LogError("doorTransform is null. Please check the setup.");
            return;
        }

        Debug.Log("Update called. isOpening: " + isOpening);

        //player triggers door by stepping into box collider trigger
        if (isOpening)
        {
            //open the door
            doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, openPosition, Time.deltaTime * slideSpeed);
            Debug.Log("Door is opening. Current position: " + doorTransform.localPosition);
        }
        else
        {
            doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedPosition, Time.deltaTime * slideSpeed);
            Debug.Log("Door is closing. Current position: " + doorTransform.localPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.tag + " | Object: " + other.name);
        if (other.CompareTag("Player"))
        {
            isOpening = true; // open the door if the player triggers event
            Debug.Log("Player entered trigger. Opening door.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.tag + " | Object: " + other.name);
        if (other.CompareTag("Player"))
        {
            isOpening = false; // when player moves away, close the door.
            Debug.Log("Player exited trigger. Closing door.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger stay detected by: " + other.tag + " | Object: " + other.name);
        if (other.CompareTag("Player"))
        {
            isOpening = true; //Ensure the door stays open while the player is on trigger
            Debug.Log("Player staying in trigger.");
        }
    }
}
        
    
