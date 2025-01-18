using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour, IDamageable
{
    [Header("---- Stats ----")]
    [SerializeField] float HP;

    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] GameObject partsPrefab;
    [SerializeField] GameObject nodePrefab;

    Color origColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed()); // Call flash red when damage is taken

        if (HP <= 0)
        {
            Instantiate(partsPrefab, transform.position, Quaternion.identity); // Drops the parts currency when the enemy is destoryed
            Instantiate(nodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.1f); //Turns red for 1 second
        model.material.color = origColor;
    }
}
