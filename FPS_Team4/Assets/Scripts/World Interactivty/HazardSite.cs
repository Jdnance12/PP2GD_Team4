// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSite : MonoBehaviour
{
    private ElectricalHazardNotification notification; // Reference to notification
    private ElectricalHazardDamage damage; // Reference to damage system

    private void Awake()
    {
        notification = GetComponentInChildren<ElectricalHazardNotification>(); // Look for components on this object and its children
        damage = GetComponentInChildren<ElectricalHazardDamage>();

        if (notification == null || damage == null)
        {
            Debug.LogWarning($"{gameObject.name}: Missing hazard components!"); // Warn if components are missing
        }

        HazardRegistry.RegisterHazard(gameObject); // Register this hazard site
        Debug.Log($"Hazard registered: {gameObject.name}"); // Log registration
    }

    private void OnDestroy()
    {
        HazardRegistry.UnregisterHazard(gameObject); // Unregister this hazard site
    }

    public void ToggleHazard(bool state)
    {
        notification?.ToggleNotification(state); // Toggle both notification and damage systems
        damage?.ToggleHazard(state);
    }
}