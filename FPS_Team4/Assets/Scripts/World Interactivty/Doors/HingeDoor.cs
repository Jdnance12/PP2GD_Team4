//MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform hinge; // Assign the Hinge child object in Inspector
    public float rotationAngle = -90f; // Rotation amount on Y-axis
    public float rotationSpeed = 2f; // Speed of door rotation

    [Header("Collider Reference")]
    public Collider doorTriggerCollider; // Assign the proper trigger collider

    [Header("UI Reference")]
    public GameObject InteractCanvas; // Reference to UI element

    private bool playerInTrigger = false; // Track if player is in the trigger
    private bool isOpen = false; // Track door state
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Coroutine rotationCoroutine;

    void Start()
    {
        if (hinge == null) // Ensure the hinge is assigned
        {
            Debug.LogError("Hinge reference missing.");
        }

        if (doorTriggerCollider == null) // Ensure the trigger collider is assigned
        {
            doorTriggerCollider = GetComponent<Collider>(); // Auto-assign collider
            if (doorTriggerCollider == null)
            {
                Debug.LogError("No trigger collider assigned to Hinge Door.");
            }
            else
            {
                doorTriggerCollider.isTrigger = true; // Ensure it's set as a trigger
            }
        }
        
        initialRotation = hinge.rotation; // Store initial rotation

        if (InteractCanvas != null) // Ensure UI text is assigned
        {
            InteractCanvas.SetActive(false); // Ensure it starts disabled
        }
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.T))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (rotationCoroutine != null) return; // Prevent multiple animations at the same time

        if (!isOpen)
        {
            targetRotation = Quaternion.Euler(hinge.eulerAngles.x, hinge.eulerAngles.y + rotationAngle, hinge.eulerAngles.z);
        }
        else
        {
            targetRotation = initialRotation; // Reset to original position
        }

        isOpen = !isOpen;

        if (isOpen && InteractCanvas != null) // Ensure UI prompt never returns after opening
        {
            InteractCanvas.SetActive(false); // Hide UI prompt permanently after door is opened
        }

        rotationCoroutine = StartCoroutine(RotateDoor(targetRotation));
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        while (Quaternion.Angle(hinge.rotation, target) > 0.1f)
        {
            hinge.rotation = Quaternion.Lerp(hinge.rotation, target, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        
        hinge.rotation = target; // Ensure exact final position
        rotationCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen) // Only show text if door is closed
        {
            playerInTrigger = true;

            if (InteractCanvas != null) 
            {
                InteractCanvas.SetActive(true); // Show UI prompt
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;

            if (InteractCanvas != null) 
            {
                InteractCanvas.SetActive(false); // Hide UI prompt
            }
        }
    }
}