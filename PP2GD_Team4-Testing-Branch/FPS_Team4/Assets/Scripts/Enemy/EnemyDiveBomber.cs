using System.Collections;
using UnityEngine;

public class EnemyDiveBomber : MonoBehaviour, IDamageable
{
    [Header("Dive Bomber Settings")]
    public float flySpeed = 5f;
    public float diveSpeed = 10f;
    public float diveRange = 8f;
    public int damage = 20;
    public int health = 30;

    private Transform player;
    private Vector2 diveTarget;
    private bool isDiving = false;
    private bool hasEnteredScreen = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No player object found in the scene! Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (!hasEnteredScreen)
        {
            FlyInFromSide();
        }
        else if (!isDiving && player != null && Vector2.Distance(transform.position, player.position) <= diveRange)
        {
            StartDive();
        }
    }

    private void FlyInFromSide()
    {
        transform.Translate(Vector2.left * flySpeed * Time.deltaTime);

        if (transform.position.x <= 10f && transform.position.x >= -10f)
        {
            hasEnteredScreen = true;
        }
    }

    private void StartDive()
    {
        isDiving = true;
        diveTarget = player.position;
        StartCoroutine(PerformDive());
    }

    private IEnumerator PerformDive()
    {
        while (Vector2.Distance(transform.position, diveTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);
            yield return null;
        }

        if (Vector2.Distance(transform.position, diveTarget) <= 0.1f)
        {
            FlyOut();
        }
    }

    private void FlyOut()
    {
        Vector2 flyOutDirection = new Vector2(-1f, 1f).normalized;
        StartCoroutine(FlyOffScreen(flyOutDirection));
    }

    private IEnumerator FlyOffScreen(Vector2 direction)
    {
        while (transform.position.x > -15f && transform.position.y < 15f)
        {
            transform.Translate(direction * flySpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDiving && collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
