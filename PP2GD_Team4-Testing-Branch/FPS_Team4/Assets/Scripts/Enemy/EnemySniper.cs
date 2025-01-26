using System.Collections;
using UnityEngine;

public class EnemySniper : MonoBehaviour, IDamageable
{
    [Header("Sniper Settings")]
    public float moveSpeed = 1f;
    public float maxSightRange = 10f;
    public float aimDuration = 2f;
    public int damage = 50;
    public float bulletSpeed = 15f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform player;
    private LineRenderer aimLine;
    private bool isAiming = false;
    private bool isAggressive = false;
    private int health = 20;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        aimLine = GetComponent<LineRenderer>();
        if (player == null)
        {
            Debug.LogError("No player object found in the scene! Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player != null && !isAiming)
        {
            CheckLineOfSight();
        }
    }

    private void CheckLineOfSight()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        if (directionToPlayer.magnitude <= maxSightRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                isAggressive = true;
                StartCoroutine(AimAndFire());
            }
        }
    }

    private IEnumerator AimAndFire()
    {
        isAiming = true;

        aimLine.enabled = true;
        aimLine.SetPosition(0, firePoint.position);
        aimLine.SetPosition(1, player.position);

        yield return new WaitForSeconds(aimDuration);

        FireBullet();

        aimLine.enabled = false;

        isAiming = false;
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
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
