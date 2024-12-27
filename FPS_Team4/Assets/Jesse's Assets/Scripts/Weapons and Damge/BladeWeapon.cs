using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BladeWeapon : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] Collider bladeCollider;

    [SerializeField] public Upgrade_Menu upgradeMenu;

    private void Start()
    {
        bladeCollider.enabled = false;
    }

    private void Update()
    {
        damage = upgradeMenu.bladeDamage;
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

        Enemy_AI enemy = other.GetComponent<Enemy_AI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
