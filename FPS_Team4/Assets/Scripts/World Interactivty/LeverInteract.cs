// MK File
using System.Collections;
using UnityEngine;

public class LeverInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LeverNotification leverNotification; // Notification system for the player
    [SerializeField] private Transform leverTransform; // Transform for the lever arm

    [Header("Hazard Components")]
    [SerializeField] private ElectricalHazardManager hazardManager; // Reference to the local manager

    [Header("Lever Settings")]
    public float leverUpRotation = 30f; // Lever rotation when in up position
    public float leverDownRotation = -20f; // Lever rotation when in down position

    private bool isLeverUp = true; // Tracks if the lever is in the up position

    private void Start()
    {
        // Initialize the lever's position to up
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
        leverTransform.localRotation = Quaternion.Euler(
            isLeverUp ? leverUpRotation : leverDownRotation,
            leverTransform.localRotation.eulerAngles.y,
            leverTransform.localRotation.eulerAngles.z
        );

        // Use groupTag instead of the hierarchy
        string groupTag = "LeverBase"; // Replace with the correct tag for this lever group
        ElectricalHazardManager.Instance.ToggleHazardsInGroup(groupTag, isLeverUp); // Toggle hazards by group
    }
}