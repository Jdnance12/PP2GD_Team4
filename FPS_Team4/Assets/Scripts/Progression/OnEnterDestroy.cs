using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
