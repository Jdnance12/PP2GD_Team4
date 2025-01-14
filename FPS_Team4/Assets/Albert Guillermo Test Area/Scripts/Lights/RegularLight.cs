using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularLight : MonoBehaviour
{
    public GameObject lightObject; // reference to the child light object

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Light lightComponent = lightObject.GetComponent<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = true;
                Debug.Log("Light turned on.");
            }
            else
            {
                Debug.LogError("No light component found on the lightObject.");
            }
        }
    }
}
