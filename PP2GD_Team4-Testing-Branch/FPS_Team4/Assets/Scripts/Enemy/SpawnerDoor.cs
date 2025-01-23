using UnityEngine;
using System.Collections;

public class SpawnerDoor : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    public float spawnInterval = 3f;

    private bool isDestroyed = false;

    private bool isSpawning = true;

    public Transform spawnPoint;

    public GameObject destructionEffect;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnPoint != null && enemyPrefabs.Length > 0)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning && !isDestroyed)
        {
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);

            Instantiate(enemyPrefabs[enemyIndex], spawnPoint.position, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void DestroyDoor()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;

            StopCoroutine(SpawnEnemies());

            if (destructionEffect != null)
            {
                Instantiate(destructionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DestroyDoor(); 
        }
    }
}
