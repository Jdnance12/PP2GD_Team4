// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalHazardDamage : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private float damagePerSecond = 40f; // Damage applied per second
    [SerializeField] private ParticleSystem[] hazardEffects; // Visual effects for sparks/arcs
    [SerializeField] private AudioSource hazardSound; // Looping hazard sound (FUTURE ITERATION)

    private bool isActive = true; // Tracks if damage system is active

    private void Start()
    {
        ToggleHazard(isActive); // Initialize hazard state
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return; // Skip if inactive

        var damageable = other.GetComponent<IDamageable>(); // Check if object implements IDamageable
        if (damageable != null)
        {
            damageable.TakeDamage(damagePerSecond * Time.deltaTime); // Apply damage
        }
    }

    public void ToggleHazard(bool state)
    {
        isActive = state; // Enable or disable hazard system

        foreach (var effect in hazardEffects)
        {
            if (state) effect.Play(); // Play effects if active
            else effect.Stop(); // Stop effects if inactive
        }

        if (hazardSound != null)
        {
            if (state) hazardSound.Play(); // Play sound if active
            else hazardSound.Stop(); // Stop sound if inactive
        }
    }
}