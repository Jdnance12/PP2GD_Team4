// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalHazardDamage : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private float damagePerSecond = 40f; // Damage applied per second
    [SerializeField] private ParticleSystem[] hazardEffects; // Visual effects for sparks/arcs
    [SerializeField] private AudioSource hazardSound; // Looping hazard sound
    [SerializeField] private Collider hazardCollider; // Hazard trigger collider

    private bool isActive = true; // Tracks if damage system is active

    private void Start()
    {
        ToggleHazard(isActive); // Initialize hazard state
        HazardRegistry.RegisterHazard(gameObject); // Register this hazard's GameObject
    }

    private void OnDestroy()
    {
        HazardRegistry.UnregisterHazard(gameObject); // Remove hazard from the registry
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return; // Skip if inactive

        var damageable = other.GetComponent<IDamageable>(); // Check if object implements IDamageable
        if (damageable != null)
        {
            damageable.TakeDamage(damagePerSecond * Time.deltaTime); // Apply damage
            Debug.Log($"Applied damage to {other.name}"); // Log damage application
        }
    }

    public void ToggleHazard(bool state)
    {
        isActive = state; // Enable or disable hazard system

        foreach (var effect in hazardEffects) // Toggle visual effects
        {
            if (effect == null) continue; // Skip if null

            if (state)
            {
                effect.Play(); // Start effect
                Debug.Log($"Effect {effect.name} turned on."); // Log effect activation
            }
            else
            {
                effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop effect
                Debug.Log($"Effect {effect.name} turned off."); // Log effect deactivation
            }
        }

        if (hazardSound != null) // Toggle sound
        {
            if (state)
            {
                hazardSound.Play(); // Play sound
                Debug.Log($"Sound {hazardSound.clip.name} played."); // Log sound activation
            }
            else
            {
                hazardSound.Stop(); // Stop sound
                Debug.Log("Sound stopped."); // Log sound deactivation
            }
        }

        // Toggle collider
        if (hazardCollider != null)
        {
            hazardCollider.enabled = state; // Enable/disable collider
            Debug.Log($"Collider {hazardCollider.name} enabled: {state}"); // Log collider state
        }

        Debug.Log($"Hazard state updated: {gameObject.name}, Active = {state}"); // Log final state
    }
}