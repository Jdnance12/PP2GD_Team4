using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPWeapon : MonoBehaviour
{

    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public Transform firePoint;
    [SerializeField] public float destroyTimer;
    [SerializeField] float bulletSpeed;
    [SerializeField] float diruptionRadious;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position,firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletSpeed;

        Destroy(bullet, destroyTimer);
    }
}
