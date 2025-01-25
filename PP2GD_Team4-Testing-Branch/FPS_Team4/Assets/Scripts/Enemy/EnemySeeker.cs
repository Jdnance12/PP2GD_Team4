using System.Collections;
using UnityEngine;

public class EnemySeeker : MonoBehaviour, IDamageable
{
    [Header("Seeker Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    public float idleTime = 1f;
    public int damage = 10;

    private Transform player;
    private Rigidbody2D rb;
    private bool isMoving = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError("No player object found in the scene! Make sure the player has the 'Player' tag.");
        }

        StartCoroutine(IdleBeforeMoving());
    }

    private IEnumerator IdleBeforeMoving()
    {
        yield return new WaitForSeconds(idleTime);
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (isMoving && player != null)
        {
            SeekPlayer();
        }
    }

    private void SeekPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.velocity = transform.right * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
