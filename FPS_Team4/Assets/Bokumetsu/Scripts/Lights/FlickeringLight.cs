using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light flickerLight;
    public GameObject lightObject; // reference to child light object
    public float flickerSpeed = 0.1f; // Time between flickers
    public float flickerDuration = 2.0f; // total time to flicker before staying on
    public bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("FlickeringLight script started.");
        if (lightObject != null)
        {
            flickerLight = lightObject.GetComponent<Light>();
            if (flickerLight == null)
            {
                Debug.LogError("No light component found on this GameObject.");
            }
            else
            {
                flickerLight.enabled = false; // light starts turned off
                Debug.Log("Light component found and set to off.");
            }
        }
        else
        {
            Debug.LogError("LightObject reference is missing.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if(!isActivated && other.CompareTag("Player"))
        {
            Debug.Log("Player detected, starting lightflicker coroutine.");
            StartCoroutine(LightFlicker());
        }
        else
        {
            Debug.Log("Non-player object detected or light already activated.");
        }
    }

    private IEnumerator LightFlicker()
    {
        Debug.Log("LightFlicker coroutine started.");
        float elapsedTime = 0f;
        isActivated = true; // mark the light as activated

        while (elapsedTime < flickerDuration)
        {
            flickerLight.enabled = !flickerLight.enabled;
            Debug.Log("Light toggled to: " + flickerLight.enabled);
            yield return new WaitForSeconds(flickerSpeed);
            elapsedTime += flickerSpeed;
        }

        // after flickering, keep the light on permanently
        flickerLight.enabled = true;
        Debug.Log("Light is now permanently on.");
    }
}
