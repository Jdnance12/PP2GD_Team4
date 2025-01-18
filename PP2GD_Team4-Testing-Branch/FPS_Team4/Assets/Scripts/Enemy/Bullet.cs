using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] public int damage;

    [SerializeField] Rigidbody rb;

    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    private void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if(damageable != null )
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    public void SetDamage(float damageModifier)
    {
        damage = Mathf.RoundToInt(damage * damageModifier);
    }
}
