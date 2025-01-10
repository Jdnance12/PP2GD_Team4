using System;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    public float speed = 20f;      // Speed of the beam
    public float lifetime = 2f;   // How long the beam exists before being destroyed
    public float damage;          // Damage of the beam (set dynamically)

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the beam forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void Initialize(float beamDamage)
    {
        damage = beamDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}

internal class EnemyHealth
{
    internal void TakeDamage(float damage)
    {
       
    }
}