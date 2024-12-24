using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BladeWeapon : MonoBehaviour
{
    [SerializeField] public int damage;

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
