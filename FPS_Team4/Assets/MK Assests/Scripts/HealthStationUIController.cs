using System.Collections;
using UnityEngine;

public class HealthStationUIController : MonoBehaviour
{
    [SerializeField] private GameObject healthStationUI; // Reference to the HealthStationUI GameObject
    private bool playerInRange = false; // Track if the player is inside the sphere collider

    void Start()
    {
        // Step 1: Find the HealthStationUI in the scene if not assigned in the inspector
        if (healthStationUI == null)
        {
            healthStationUI = GameObject.Find("HealthStationUI");
        }

        if (healthStationUI != null)
        {
            Debug.Log("HealthStationUI found successfully.");
            healthStationUI.SetActive(false); // Ensure it's off initially
        }
        else
        {
            Debug.LogError("HealthStationUI not found in the scene. Check the name and hierarchy.");
        }
    }

    void Update()
    {
        // Step 2: Check if the player presses "T" while inside the trigger zone
        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            ShowHealthStationUI(); // Show the UI message for 3 seconds
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Step 3: Detect player entering the Sphere Collider
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered heal station trigger.");
            playerInRange = true; // Player is now in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Step 4: Detect player exiting the Sphere Collider
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited heal station trigger.");
            playerInRange = false; // Player is no longer in range
        }
    }

    public void ShowHealthStationUI()
    {
        // Step 5: Show the UI for 3 seconds
        if (healthStationUI != null)
        {
            StartCoroutine(ToggleUI());
        }
        else
        {
            Debug.LogError("Cannot show UI. HealthStationUI reference is NULL.");
        }
    }

    IEnumerator ToggleUI()
    {
        Debug.Log("Activating HealthStationUI...");
        healthStationUI.SetActive(true); // Turn on the UI

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        Debug.Log("Deactivating HealthStationUI...");
        healthStationUI.SetActive(false); // Turn off the UI
    }
}
