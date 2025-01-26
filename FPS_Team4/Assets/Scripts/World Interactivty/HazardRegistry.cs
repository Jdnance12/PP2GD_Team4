// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HazardRegistry
{
    public static List<GameObject> HazardSites = new List<GameObject>(); // Central hazard list


    public static void RegisterHazard(GameObject hazardSite)
    {
        if (!HazardSites.Contains(hazardSite)) // Prevent duplicates
        {
            HazardSites.Add(hazardSite); // Add new hazard site
            Debug.Log($"Registered Hazard: {hazardSite.name}. Total hazards: {HazardSites.Count}"); // Log registration
        }
    }

    public static void UnregisterHazard(GameObject hazardSite)
    {
        if (HazardSites.Contains(hazardSite)) // Check if hazard exists
        {
            HazardSites.Remove(hazardSite); // Remove hazard site
            Debug.Log($"Unregistered Hazard: {hazardSite.name}. Total hazards: {HazardSites.Count}"); // Log unregistration
        }
    }

    // Check for duplicate tags among registered hazards
    public static void ValidateUniqueTags()
    {
        Dictionary<string, List<GameObject>> tagMap = new Dictionary<string, List<GameObject>>();

        // Group hazards by tags
        foreach (var hazard in HazardSites)
        {
            string tag = hazard.tag;

            if (!tagMap.ContainsKey(tag))
            {
                tagMap[tag] = new List<GameObject>();
            }

            tagMap[tag].Add(hazard);
        }

        // Log duplicate tags
        foreach (var entry in tagMap)
        {
            if (entry.Value.Count > 1) // More than one object with the same tag
            {
                Debug.LogWarning($"Duplicate Tag Detected: {entry.Key}. Objects sharing this tag:");

                foreach (var obj in entry.Value)
                {
                    Debug.LogWarning($" - {obj.name} (Position: {obj.transform.position})");
                }

                Debug.LogWarning($"Please assign unique tags for these objects for each site. It is coded to only look for unique tags for each Base.");
            }
        }
    }
}