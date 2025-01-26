// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HazardRegistry
{
    private static Dictionary<string, List<GameObject>> siteHazards = new Dictionary<string, List<GameObject>>(); // Stores hazards by site

    public static void RegisterHazard(GameObject hazardSite)
    {
        string siteName = GetParentSiteName(hazardSite); // Get the parent site name

    if (siteName == null)
    {
        Debug.LogWarning($"Hazard {hazardSite.name} has no valid parent site!"); // Warn if no parent site
        return;
    }

    if (!siteHazards.ContainsKey(siteName))
    {
        siteHazards[siteName] = new List<GameObject>(); // Create a new list for this site
    }

    if (!siteHazards[siteName].Contains(hazardSite)) // Prevent duplicates
    {
        siteHazards[siteName].Add(hazardSite); // Add hazard to the site
        Debug.Log($"Registered Hazard: {hazardSite.name} under {siteName}. Total hazards: {siteHazards[siteName].Count}");
    }
    }

    public static void UnregisterHazard(GameObject hazardSite)
    {
        string siteName = GetParentSiteName(hazardSite); // Get the parent site name

        if (siteName != null && siteHazards.ContainsKey(siteName))
        {
            siteHazards[siteName].Remove(hazardSite); // Remove hazard from the site
            Debug.Log($"Unregistered Hazard: {hazardSite.name} under {siteName}. Remaining hazards: {siteHazards[siteName].Count}");

            if (siteHazards[siteName].Count == 0)
            {
                siteHazards.Remove(siteName); // Remove site if no hazards remain
            }
        }
    }

    public static List<GameObject> GetHazardsBySite(string siteName)
    {
        if (siteHazards.ContainsKey(siteName))
        {
            return siteHazards[siteName]; // Return hazards for the site
        }

        Debug.LogWarning($"No hazards found for site {siteName}"); // Warn if no hazards found
        return new List<GameObject>();
    }

    public static string GetParentSiteName(GameObject hazard)
    {
        Transform current = hazard.transform; // Start from the hazards transform

        while (current.parent != null) // Go up the hierarchy to find the root parent
        {
            current = current.parent;
        }

        return current.name; // Return the root objects name
    }
}