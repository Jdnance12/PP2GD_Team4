using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpotlightDetection : MonoBehaviour
{
    [SerializeField] Light spotlight;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float detectionRange;
    [SerializeField] Collider spotlightTrigger;

    public delegate void PlayerDetectedHandler();
    public event PlayerDetectedHandler OnPlayerDetected;

    // Start is called before the first frame update
    void Start()
    {
        spotlight.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            NotifyNearbyEnemies();
            OnPlayerDetected?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    void NotifyNearbyEnemies()
    {

    }
}
