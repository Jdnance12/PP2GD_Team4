using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{

    [SerializeField] public int damage;
    [SerializeField] public float range;
    [SerializeField] public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        if(Physics.Raycast(rayOrigin, rayDirection, out hit, range)) 
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        Debug.DrawRay(rayOrigin, rayDirection * range, Color.red, 1.0f);
    }
}
