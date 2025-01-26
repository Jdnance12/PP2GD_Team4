// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : MonoBehaviour
{
    // [Header("Electrical Hazard Components")]
    // [SerializeField] private ElectricalHazardNotification notificationSystem; // Notification script
    // [SerializeField] private ElectricalHazardDamage damageSystem; // Damage script

    [Header("Group Settings")]
    [SerializeField] private string groupTag; // Identifier for the hazard group this switch controls

    private bool isSystemActive = true; // Tracks if the system is active

    public void ToggleBreaker()
    {
        isSystemActive = !isSystemActive; // Toggle system state
        Debug.Log($"Breaker toggled. New state: {isSystemActive}"); // Log state toggle

        foreach (var hazardSite in HazardRegistry.HazardSites)
        {
            if (hazardSite.CompareTag(groupTag)) // Match hazard with the groupTag
            {
                hazardSite.GetComponent<HazardSite>()?.ToggleHazard(isSystemActive); // Toggle hazard state
            }
        }
    }
}