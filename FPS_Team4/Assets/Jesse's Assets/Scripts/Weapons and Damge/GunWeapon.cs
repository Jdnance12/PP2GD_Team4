using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    [Header("---- Weapon Components ----")]
    [SerializeField] public Camera playerCamera;
    [SerializeField] public GameObject hitEffectPrefab;
    [SerializeField] public GameObject muzzleFlashPrefab;
    [SerializeField] public Transform muzzleFlashPoint;

    [Header("---- Weapon Stats ----")]
    [SerializeField] public int damage;
    [SerializeField] public float range;
    [SerializeField] public float shootRate;
    [SerializeField] public float accuracy;
    [SerializeField] public float effectDuration;
    [SerializeField] public float muzzleFlashDuration;

    [Header("---- Bools ----")]
    bool isFiring = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = GetShootDirection();

        if(Physics.Raycast(rayOrigin, rayDirection, out hit, range)) 
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            Destroy(muzzleFlash, muzzleFlashDuration);

            GameObject effect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(effect, effectDuration);
        }

        Debug.DrawRay(rayOrigin, rayDirection * range, Color.red, 1.0f);
    }

    private Vector3 GetShootDirection()
    {
        Vector3 dir = playerCamera.transform.forward;
        dir.x += UnityEngine.Random.Range(-accuracy, accuracy);
        dir.y += UnityEngine.Random.Range(-accuracy, accuracy);
        dir.z += UnityEngine.Random.Range(-accuracy, accuracy);

        return dir.normalized;
    }

    public IEnumerator FireCoroutine()
    {
        isFiring = true;

        while (isFiring)
        {
            Shoot();
            yield return new WaitForSeconds(shootRate);
        }
    }
}
