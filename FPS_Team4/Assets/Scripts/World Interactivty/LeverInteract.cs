// MK File
using System.Collections;
using UnityEngine;

public class LeverInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LeverNotification leverNotification; // Notification system for the player
    [SerializeField] private Transform leverTransform; // Transform for the lever arm
    [SerializeField] private AudioSource clickSound; // Audio source for the click sound

    [Header("Lever Settings")]
    public float leverUpRotation = 30f; // Lever rotation when in up position
    public float leverDownRotation = -20f; // Lever rotation when in down position

    private bool isLeverUp = true; // Tracks if the lever is in the up position

    private void Start()
    {
        // Initialize the levers position to up
        leverTransform.localRotation = Quaternion.Euler(
            leverUpRotation,
            leverTransform.localRotation.eulerAngles.y,
            leverTransform.localRotation.eulerAngles.z
        );
    }

    private void Update()
    {
        // Check if player is in range and presses "T"
        if (leverNotification.IsPlayerInRange() && Input.GetKeyDown(KeyCode.T))
        {
            ToggleLever(); // Flip lever state
        }
    }

    public void ToggleLever()
    {
        isLeverUp = !isLeverUp; // Toggle lever state

        // Update the lever's rotation
        leverTransform.localRotation = Quaternion.Euler(
            isLeverUp ? leverUpRotation : leverDownRotation,
            leverTransform.localRotation.eulerAngles.y,
            leverTransform.localRotation.eulerAngles.z
        );

        // Play the click sound for both on and off states
        if (clickSound != null)
        {
            clickSound.Play(); // Play the sound
            Debug.Log("Click sound played.");
        }
        // Get the top-level parent name using HazardRegistry
        string parentSiteName = HazardRegistry.GetParentSiteName(gameObject);

        foreach (var hazard in HazardRegistry.GetHazardsBySite(parentSiteName))
        {
            hazard.GetComponent<HazardSite>()?.ToggleHazard(isLeverUp); // Toggle hazard state
        }
    }
}