using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 vOpenPosition;
    private Vector3 closedPosition;
    [SerializeField] private Vector3 brokenPosition;
    [SerializeField] private Vector3 vBrokenPosition;

    [SerializeField] private float slideSpeed = 1.0f; // Speed of the sliding door

    public bool isOpening = false;
    [SerializeField] private bool isBroken;
    [SerializeField] public bool isVerticalDoor = false;

    private int tryDoorCount = 0;

    private Transform doorTransform; // Reference to the door's transform

    // Start is called before the first frame update
    void Start()
    {
        // Check if child was placed correctly
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
            Debug.LogError("doorTransform is null on GameObject: " + gameObject.name + ". Please check the setup.");
            return;
        }
        // if not opening, uncomment below to check if isOpening is working
        //Debug.Log("Update called. isOpening: " + isOpening);

        //player triggers door by stepping into box collider trigger
        if (!isVerticalDoor)
        {
            if (isOpening)
            {
                //open the door
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, openPosition, Time.deltaTime * slideSpeed);
                // uncomment below if not detecting player, otherwise this is redundant
                //Debug.Log("Door is opening. Current position: " + doorTransform.localPosition);
            }
            else
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedPosition, Time.deltaTime * slideSpeed);
                // uncomment below if door is being weird
                //Debug.Log("Door is closing. Current position: " + doorTransform.localPosition);
            }

            if (isBroken && tryDoorCount > 0)
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, brokenPosition, Time.deltaTime * slideSpeed);
            }
        }
        else
        {
            if (isOpening)
            {
                //open the door
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, vOpenPosition, Time.deltaTime * slideSpeed);
                // uncomment below if not detecting player, otherwise this is redundant
                //Debug.Log("Door is opening. Current position: " + doorTransform.localPosition);
            }
            else
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedPosition, Time.deltaTime * slideSpeed);
                // uncomment below if door is being weird
                //Debug.Log("Door is closing. Current position: " + doorTransform.localPosition);
            }

            if (isBroken && tryDoorCount > 0)
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, vBrokenPosition, Time.deltaTime * slideSpeed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.tag + " | Object: " + other.name);
        if (other.CompareTag("Player"))
        {
            // if door is not broken
            if (!isBroken)
            {
                isOpening = true; // open the door if the player triggers event
                Debug.Log("Player entered trigger. Opening door.");
            }

            // if door is broken
            else
            {
                tryDoorCount = 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.tag + " | Object: " + other.name);
        if (other.CompareTag("Player"))
        {
            // if door is not broken
            if (!isBroken)
            {
                isOpening = false; // when player moves away, close the door.
                Debug.Log("Player exited trigger. Closing door.");
            }
        }
    }

    ////uncomment below if door is closing when player is inside trigger
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("Trigger stay detected by: " + other.tag + " | Object: " + other.name);
    //    if (other.CompareTag("Player"))
    //    {
    //        // if door is not broken
    //        if (!isBroken)
    //        {
    //            isOpening = true; //Ensure the door stays open while the player is on trigger
    //            Debug.Log("Player staying in trigger.");
    //        }
    //    }
    //}
}
        
    
