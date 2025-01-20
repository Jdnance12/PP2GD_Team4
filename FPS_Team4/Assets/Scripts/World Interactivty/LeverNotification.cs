// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Supports TextMeshPro for UI text

public class LeverNotification : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text notificationText; // Text for player instructions
    [SerializeField] private GameObject notificationCanvas; // Notification canvas

    private bool isPlayerInRange = false; // Tracks if player is near lever

    private void Start()
    {
        if (notificationCanvas != null)
            notificationCanvas.SetActive(false); // Hide notification at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detects if player enters trigger
        {
            isPlayerInRange = true; // Mark player in range
            if (notificationText != null) 
                notificationText.text = "Press 'T' to toggle the breaker"; // Instruction message
            if (notificationCanvas != null)
                notificationCanvas.SetActive(true); // Show notification
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detects if player exits trigger
        {
            isPlayerInRange = false; // Mark player out of range
            if (notificationCanvas != null)
                notificationCanvas.SetActive(false); // Hide notification
        }
    }

    public bool IsPlayerInRange() // Check if player is in range
    {
        return isPlayerInRange; // Returns true if player is nearby
    }
}