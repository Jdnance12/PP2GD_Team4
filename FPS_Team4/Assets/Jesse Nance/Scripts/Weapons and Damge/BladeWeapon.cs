using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem.HID;

public class BladeWeapon : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] Collider bladeCollider;

    [SerializeField] public GameObject upgradeObject;
    [SerializeField] public UpgradeScript upgradeScript;

    private void Start()
    {
        bladeCollider.enabled = false;

        upgradeObject = GameObject.Find("Game Manager");
        upgradeScript = upgradeObject.GetComponent<UpgradeScript>();
    }

    private void Update()
    {
        //damage = upgradeMenu.GetUpgradedBladeDamage();
    }

    public void EnableCollider()
    {
        bladeCollider.enabled = true;
    }

    public void DisableCollider()
    {
        bladeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}
