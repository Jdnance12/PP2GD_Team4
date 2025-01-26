// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : MonoBehaviour
{
    private bool isSystemActive = true; // Tracks if the system is active

    public void ToggleBreaker()
    {
        isSystemActive = !isSystemActive; // Toggle system state
        Debug.Log($"Breaker toggled. New state: {isSystemActive}"); // Log state toggle

        string parentSiteName = transform.parent.name; // Get the parent site name

        if (string.IsNullOrEmpty(parentSiteName)) // Check if parent name exists
        {
            Debug.LogWarning($"Breaker {name} has no parent site."); // Log warning
            return;
        }

        foreach (var hazard in HazardRegistry.GetHazardsBySite(parentSiteName))
        {
            hazard.GetComponent<HazardSite>()?.ToggleHazard(isSystemActive); // Toggle hazard state
            Debug.Log($"Toggled hazard: {hazard.name} under site: {parentSiteName}. State: {isSystemActive}");
        }
    }
}