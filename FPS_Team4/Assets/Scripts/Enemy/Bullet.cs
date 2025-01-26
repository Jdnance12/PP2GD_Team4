using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] public int damage;

    [SerializeField] Rigidbody rb;

    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    private bool isEnemyBullet = false; // Flag to indicate a bullet is an Enemy bullet.
    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Debug.Log("Bullet velocity set to: " + rb.velocity);
        }
        else
        {
            Debug.LogWarning("Rigidbody not found on bullet.");
        }
        
        Destroy(gameObject, destroyTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if(isEnemyBullet && other.CompareTag("Enemy"))
        {
            // Ignore collision with other enemies
            return;
        }

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if(damageable != null )
        {
            Debug.Log("Bullet hit: " + other.gameObject.name);
            damageable.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Bullet hit non-damagable object: " + other.gameObject.name);
        }

        Destroy(gameObject);
    }

    public void SetDamage(float damageModifier)
    {
        damage = Mathf.RoundToInt(damage * damageModifier);
    }

    public void SetIsEnemyBullet(bool isEnemy)
    {
        isEnemyBullet = isEnemy;
    }
}
