// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalHazardManager : MonoBehaviour
{
    private static ElectricalHazardManager instance; // Singleton instance

    [Header("Hazards")]
    [SerializeField] private List<ElectricalHazardDamage> hazards; // All hazards in the scene

    private void Awake()
    {
        if (instance == null) instance = this; // Assign singleton instance
        else Destroy(gameObject); // Prevent duplicate managers
    }

    public static ElectricalHazardManager Instance => instance; // Accessor for manager instance

    private void Start()
    {
        if (hazards == null || hazards.Count == 0) // If no hazards manually assigned
        {
            hazards = new List<ElectricalHazardDamage>(
                GetComponentsInChildren<ElectricalHazardDamage>() // Find hazards within this object
            );
        }

        Debug.Log($"Hazard Manager initialized. Registered hazards: {hazards.Count}"); // Log hazard count
    }

    public void ToggleAllHazards(bool state) // Enable/disable all hazards
    {
        foreach (var hazard in hazards)
        {
            hazard.ToggleHazard(state); // Apply state to each hazard
            Debug.Log($"ToggleAllHazards: Toggled {hazard.name} to state {state}"); // Log toggle
        }
    }

    public void RegisterHazard(ElectricalHazardDamage hazard) // Add new hazard to manager
    {
        if (!hazards.Contains(hazard))
        {
            hazards.Add(hazard); // Avoid duplicates
            Debug.Log($"Registered hazard: {hazard.name}"); // Log hazard registration
        }
    }

    public void ToggleHazardsInGroup(string groupTag, bool state)
    {
        Debug.Log($"Toggling hazards for group: {groupTag}, State: {state}"); // Log group toggle

        // foreach (var hazard in hazards)
        // {
        //     if (hazard.GetGroupTag() == groupTag) // Match hazards by groupTag
        //     {
        //         Debug.Log($"Toggling hazard: {hazard.name} to state {state}"); // Log toggling hazard
        //         hazard.ToggleHazard(state); // Toggle hazard
        //     }
        //     else
        //     {
        //         Debug.Log($"Hazard {hazard.name} does NOT belong to group {groupTag}"); // Log hazard mismatch
        //     }
        // }
    }
}