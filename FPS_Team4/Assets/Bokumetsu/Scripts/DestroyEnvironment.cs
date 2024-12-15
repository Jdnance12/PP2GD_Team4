using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnvironment : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject objectToDeactivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Activate Object A
            if(objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                Debug.Log(objectToActivate.name + " activated.");
            }

            //Deactivate Object B
            if(objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
                Debug.Log(objectToDeactivate.name + "deactivated.");
            }
        }
    }
}
