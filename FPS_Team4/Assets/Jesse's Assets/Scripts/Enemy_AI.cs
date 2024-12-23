using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour, IDamageable, IDisrupt
{
    [Header("---- Enemy Components ----")]
    [SerializeField] Renderer model;

    Color origColor;

    [Header("---- Enemy Stats ----")]
    [SerializeField] public int HP;

    public bool isDisrupted = false;


    // Start is called before the first frame update
    void Start()
    {
        origColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        BladeWeapon blade = other.GetComponent<BladeWeapon>();

        if(blade != null)
        {
            TakeDamage(blade.damage);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            Destroy();
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }

    public void causeDisrupt()
    {
        isDisrupted = true;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
