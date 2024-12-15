using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPickUp : MonoBehaviour
{
    [SerializeField] GunStats weapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
        }
    }
}
