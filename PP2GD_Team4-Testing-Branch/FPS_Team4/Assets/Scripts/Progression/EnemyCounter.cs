using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    GameManager gameManager;
    public int counter;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }
    private void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        counter = enemies.Length;

        if(counter <= 0)
        {
            counter = 0;
            gameManager.progressScript.hallenemiesKilled = true;

        }
    }
}
