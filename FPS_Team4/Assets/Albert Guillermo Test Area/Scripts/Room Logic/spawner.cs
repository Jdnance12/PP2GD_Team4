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
    [SerializeField] private float spawnRadius = 5.0f; // radius for random spawns

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
        Vector3 randomPosition = GetRandomPosition(spawnPositions[spawnIndex].position, spawnRadius);

        GameObject newEnemy = Instantiate(enemyPrefab, randomPosition, spawnPositions[spawnIndex].rotation);
        EnemyBasic enemyBasic = newEnemy.GetComponent<EnemyBasic>();

        if(enemyBasic != null)
        {
            enemyBasic.ApplyModifiers(enemyHpModifier, enemyDamageModifier);
            enemyBasic.OnDeath += OnEnemyDeath;
        }
        spawnCount++;
        isSpawning = false;
    }

    Vector3 GetRandomPosition(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);
        Vector3 randomPosition = center + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
        return randomPosition;
    }

    void OnEnemyDeath(EnemyBasic enemy)
    {
        spawnCount--;
        if (startSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(SpawnEnemies());
        }
    }
}
