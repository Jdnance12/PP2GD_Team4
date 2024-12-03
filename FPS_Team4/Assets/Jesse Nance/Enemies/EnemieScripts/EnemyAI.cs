using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;

    [SerializeField] Renderer model;
    Color colorOrigin;

    // Start is called before the first frame update
    void Start()
    {
        colorOrigin = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flastRed());

        if(HP <= 0)
        {
            //I'm dead
            Destroy(gameObject);
        }
    }

    IEnumerator flastRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrigin;
    }
}
