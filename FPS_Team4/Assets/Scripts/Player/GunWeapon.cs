using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    [Header("---- Weapon Components ----")]
    [SerializeField] public Camera playerCamera;
    public GameObject upgradeObject;
    public UpgradeManager upgradeManager;
    [SerializeField] public GameObject hitEffectPrefab;
    [SerializeField] public GameObject muzzleFlashPrefab;
    [SerializeField] public Transform muzzleFlashPoint;
    [SerializeField] public Transform playerArm; // Reference to the player's arm

    [Header("---- Weapon Stats ----")]
    [SerializeField] public float damage;
    [SerializeField] private float currentDamage;
    [SerializeField] public float range;
    [SerializeField] public float shootRate;
    [SerializeField] public float accuracy;
    [SerializeField] public float effectDuration;
    [SerializeField] public float muzzleFlashDuration;

    [Header("---- Recoil Settings ----")]
    [SerializeField] public float recoilAmount = 0.1f; // Amount of recoil
    [SerializeField] public float recoilSpeed = 10f;
    [SerializeField] public float recoilReturnSpeed = 5f;

    [Header("---- Bools ----")]
    bool isFiring = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 armOriginalPosition;
    private Quaternion armOriginalRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        upgradeObject = GameObject.Find("Game Manager");
        upgradeManager = upgradeObject.GetComponent<UpgradeManager>();

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        if (playerArm != null)
        {
            armOriginalPosition = playerArm.localPosition;
            armOriginalRotation = playerArm.localRotation;
        }

        //currentDamage = upgradeManager.GetUpgradedGunDamage();
    }

    // Update is called once per frame
    void Update()
    {
        currentDamage = upgradeManager.GetUpgradedGunDamage();
    }

    public void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = GetShootDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, range))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(currentDamage);
            }

            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            Destroy(muzzleFlash, muzzleFlashDuration);

            GameObject effect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(effect, effectDuration);
        }

        Debug.DrawRay(rayOrigin, rayDirection * range, Color.red, 1.0f);

        // Apply visual recoil
        StartCoroutine(ApplyRecoil());
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

    private IEnumerator ApplyRecoil()
    {
        if (playerArm == null)
        {
            yield break;
        }

        Vector3 recoilOffset = new Vector3(0, 0, -recoilAmount);

        float elapsedTime = 0f;

        // Move the arm back
        while (elapsedTime < 1f / recoilSpeed)
        {
            playerArm.localPosition = Vector3.Lerp(playerArm.localPosition, armOriginalPosition + recoilOffset, elapsedTime * recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Return the arm to its original position
        while (elapsedTime < 1f / recoilReturnSpeed)
        {
            playerArm.localPosition = Vector3.Lerp(playerArm.localPosition, armOriginalPosition, elapsedTime * recoilReturnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerArm.localPosition = armOriginalPosition;
    }
}
