using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerDetection : MonoBehaviour
{
    public Light spotlight;

    public bool playerIsDetected;

    Color spotlightOriginColor;

    public Collider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        spotlightOriginColor = spotlight.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsDetected = true;
            spotlight.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsDetected = false;
            spotlight.color = spotlightOriginColor;
        }
    }
}
