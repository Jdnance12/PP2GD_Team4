// MK File
using System.Collections;
using UnityEngine;

public class LeverInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LeverNotification leverNotification; // Notification system for the player
    [SerializeField] private Transform leverTransform; // Transform for the lever arm
    [SerializeField] private ElectricalHazardNotification hazardNotification; // Notification system for hazards
    [SerializeField] private ElectricalHazardDamage hazardDamage; // Damage system for hazards

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
        isLeverUp = !isLeverUp; // Toggle lever state between up and down

        // Update the levers rotation based on its new state
        leverTransform.localRotation = Quaternion.Euler(
            isLeverUp ? leverUpRotation : leverDownRotation,
            leverTransform.localRotation.eulerAngles.y,
            leverTransform.localRotation.eulerAngles.z
        );

        // Toggle hazard systems based on lever state
        bool isActive = isLeverUp; // Hazards are active when lever is up
        hazardNotification.ToggleNotification(isActive); // Enable or disable notifications
        hazardDamage.ToggleHazard(isActive); // Enable or disable damage
    }
}