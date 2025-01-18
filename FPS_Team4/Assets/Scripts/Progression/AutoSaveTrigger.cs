//MK FILE
using System.Collections;
using UnityEngine;
using TMPro; // Supports TextMeshPro for UI text

public class AutoSaveTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text saveNotificationText; // TMP_Text component for save notification
    [SerializeField] private GameObject checkpointMarker; // Reference to CheckpointMarker object
    public float displayDuration = 2f; // How long notification stays visible
    public float pulseDuration = 0.5f; // Pulse animation duration
    public int pulseCount = 3; // Number of pulses
    private bool hasTriggered = false; // Tracks if trigger is already used

    private void Awake()
    {
        if (saveNotificationText == null) // Ensure saveNotificationText is assigned
        {
            Debug.LogError("SaveNotificationText (TMP_Text) is not assigned in the Inspector!"); // Logs error if missing
            return;
        }
        saveNotificationText.gameObject.SetActive(false); // Disables the text by default

        if (checkpointMarker == null) // Ensure checkpointMarker is assigned
        {
            Debug.LogError("CheckpointMarker is not assigned in the Inspector!"); // Logs error if missing
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // Ensures trigger is unused and collider is player
        {
            hasTriggered = true; // Marks this trigger as used
            GameManager.instance.SaveGame(); // Saves game state
            RemoveCheckpointMarker(); // Deletes the CheckpointMarker object
            ShowSaveNotification(); // Displays save notification
        }
    }

    private void RemoveCheckpointMarker()
    {
        if (checkpointMarker != null) // Ensures CheckpointMarker exists
        {
            Destroy(checkpointMarker); // Deletes the CheckpointMarker object
        }
    }

    private void ShowSaveNotification()
    {
        if (saveNotificationText == null) return; // Exits if text component is missing
        saveNotificationText.gameObject.SetActive(true); // Activates the text
        saveNotificationText.text = "Checkpoint Reached"; // Sets notification text
        StartCoroutine(PulseText()); // Triggers pulse animation
        Invoke(nameof(HideAndDisableTrigger), displayDuration); // Hides text and disables trigger
    }

    private IEnumerator PulseText()
    {
        Vector3 originalScale = saveNotificationText.transform.localScale; // Saves initial scale
        Vector3 targetScale = originalScale * 1.2f; // Sets pulse scale

        for (int i = 0; i < pulseCount; i++) // Loops for the specified number of pulses
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                saveNotificationText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration); // Scales up
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                saveNotificationText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration); // Scales down
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        saveNotificationText.transform.localScale = originalScale; // Restores original scale
    }

    private void HideAndDisableTrigger()
    {
        if (saveNotificationText != null) // Checks if text exists
            saveNotificationText.gameObject.SetActive(false); // Deactivates the text

        gameObject.SetActive(false); // Disables this trigger object
    }
}