// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : MonoBehaviour
{
    [Header("Electrical Hazard Components")]
    [SerializeField] private ElectricalHazardNotification notificationSystem; // Notification script
    [SerializeField] private ElectricalHazardDamage damageSystem; // Damage script

    [Header("Group Settings")]
    [SerializeField] private string groupTag; // Identifier for the hazard group this switch controls

    private bool isSystemActive = true; // Tracks if the system is active

    public void ToggleBreaker()
    {
        isSystemActive = !isSystemActive; // Toggle system state
        Debug.Log($"Breaker toggled. New state: {isSystemActive}"); // Log state toggle

        // Use the groupTag to toggle hazards
        ElectricalHazardManager.Instance.ToggleHazardsInGroup(groupTag, isSystemActive); // Group-specific toggle
    }
}