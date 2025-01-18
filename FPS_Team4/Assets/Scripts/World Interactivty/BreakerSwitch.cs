// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerSwitch : MonoBehaviour
{
    [Header("Electrical Hazard Components")]
    [SerializeField] private ElectricalHazardNotification notificationSystem; // Notification script
    [SerializeField] private ElectricalHazardDamage damageSystem; // Damage script

    private bool isSystemActive = true; // Tracks if system is active

    public void ToggleBreaker()
    {
        isSystemActive = !isSystemActive; // Toggle system state
        notificationSystem.ToggleNotification(isSystemActive); // Toggle notifications
        damageSystem.ToggleHazard(isSystemActive); // Toggle hazard effects and damage
    }
}