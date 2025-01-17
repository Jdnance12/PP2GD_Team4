using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int numToSpawn;
    [SerializeField] private int timeBetweenSpawns;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private float enemyHpModifier = 1.0f;
    [SerializeField] private float enemyDamageModifier = 1.0f;

    int spawnCount;
    bool startSpawning;
    bool isSpawning;

    // Start is called before the first frame update
    void Update()
    {
        if(startSpawning && spawnCount < numToSpawn && !isSpawning)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator SpawnEnemies()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBetweenSpawns);

        int spawnIndex = Random.Range(0, spawnPositions.Length);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPositions[spawnIndex].position, spawnPositions[spawnIndex].rotation);
        EnemyBasic enemyBasic = newEnemy.GetComponent<EnemyBasic>();

        if(enemyBasic != null)
        {
            enemyBasic.ApplyModifiers(enemyHpModifier, enemyDamageModifier);
        }
        spawnCount++;
        isSpawning = false;
    }
}
