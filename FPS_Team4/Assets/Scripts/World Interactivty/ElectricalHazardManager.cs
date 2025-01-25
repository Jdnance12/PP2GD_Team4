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
    }

    public void ToggleAllHazards(bool state) // Enable/disable all hazards
    {
        foreach (var hazard in hazards)
        {
            hazard.ToggleHazard(state); // Apply state to each hazard
        }
    }

    public void RegisterHazard(ElectricalHazardDamage hazard) // Add new hazard to manager
    {
        if (!hazards.Contains(hazard)) hazards.Add(hazard); // Avoid duplicates
    }

    public void ToggleHazardsInGroup(Transform groupParent, bool state)
    {
        foreach (var hazard in hazards)
        {
            if (hazard.transform.IsChildOf(groupParent)) // Only hazards within the group
            {
                hazard.ToggleHazard(state);
            }
        }
    }

}