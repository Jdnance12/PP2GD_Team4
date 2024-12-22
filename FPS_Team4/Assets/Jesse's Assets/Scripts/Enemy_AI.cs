using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour, IDamageable, IDisrupt
{

    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;

    public bool isDisrupted = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if(HP <= 0)
        {
            Destroy();
        }
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
