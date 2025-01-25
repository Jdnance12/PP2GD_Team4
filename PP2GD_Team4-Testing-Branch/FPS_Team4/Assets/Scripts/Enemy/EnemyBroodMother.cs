using System.Collections;
using UnityEngine;

public class EnemyBroodMother : MonoBehaviour, IDamageable
{
    [Header("Brood Mother Settings")]
    public float moveSpeed = 1f;
    public float spawnInterval = 3f;
    public GameObject broodMinionPrefab;
    public Transform spawnPoint;
    public float throwForce = 5f;

    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError("No player object found in the scene! Make sure the player has the 'Player' tag.");
        }

        StartCoroutine(SpawnMinions());
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    private IEnumerator SpawnMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            ThrowMinion();
        }
    }

    private void ThrowMinion()
    {
        if (broodMinionPrefab != null && player != null && spawnPoint != null)
        {
            Vector2 direction = (player.position - spawnPoint.position).normalized;

            GameObject minion = Instantiate(broodMinionPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D minionRb = minion.GetComponent<Rigidbody2D>();

            if (minionRb != null)
            {
                minionRb.AddForce((direction + Vector2.up * 0.5f).normalized * throwForce, ForceMode2D.Impulse);
            }
        }
    }
}
