using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate_Object : MonoBehaviour
{
    public GameObject objectToActivate;

    private void Start()
    {
        objectToActivate.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToActivate.SetActive(true);
        }   
    }
    private void OnTriggerExit(Collider other)
    {
        objectToActivate.SetActive(false);
    }
}
