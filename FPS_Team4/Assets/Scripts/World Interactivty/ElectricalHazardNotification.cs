// MK FILE
using System.Collections;
using UnityEngine;
using TMPro; // Supports TextMeshPro for UI text

public class ElectricalHazardNotification : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text hazardNotificationText; // Text for hazard notification
    [SerializeField] private GameObject notificationCanvas; // Parent canvas for notifications
    public float displayDuration = 2f; // Duration for notification visibility
    public float pulseDuration = 0.5f; // Pulse animation duration
    public int pulseCount = 3; // Number of pulses

    private bool isPlayerInRange = false; // Tracks if player is in notification range
    private bool isActive = true; // Tracks if notification system is active

    private void Start()
    {
        notificationCanvas.SetActive(true); // Keeps canvas active
        hazardNotificationText.gameObject.SetActive(false); // Starts with notification hidden
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return; // Skip if inactive or not player

        isPlayerInRange = true; // Player entered range
        hazardNotificationText.gameObject.SetActive(true); // Show notification
        hazardNotificationText.text = "Danger: High Voltage Hazard! System Meltdown Risk!"; // Set notification text
        StartCoroutine(PulseText()); // Start text pulse animation
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return; // Skip if inactive or not player

        isPlayerInRange = false; // Player exited range
        hazardNotificationText.gameObject.SetActive(false); // Hide notification
    }

    private IEnumerator PulseText()
    {
        Vector3 originalScale = hazardNotificationText.transform.localScale; // Original scale
        Vector3 targetScale = originalScale * 1.2f; // Target pulse scale

        for (int i = 0; i < pulseCount; i++)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                hazardNotificationText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration); // Scale up
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                hazardNotificationText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration); // Scale down
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        hazardNotificationText.transform.localScale = originalScale; // Reset scale
    }

    public void ToggleNotification(bool state)
    {
        isActive = state; // Enable or disable notification system
        if (!isActive && hazardNotificationText != null)
        {
            hazardNotificationText.gameObject.SetActive(false); // Hide notification if deactivated
        }
    }
}