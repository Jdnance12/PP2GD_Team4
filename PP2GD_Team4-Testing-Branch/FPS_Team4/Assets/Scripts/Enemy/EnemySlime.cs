using System.Collections;
using UnityEngine;

public class EnemySlime : MonoBehaviour, IDamageable
{
    [Header("Slime Settings")]
    public float hopForce = 5f;
    public int damage = 10;
    public int maxHealth = 20;
    public int minHealthForSplit = 5;
    public GameObject smallerSlimePrefab;

    private int currentHealth;
    private Rigidbody2D rb;
    private bool isHopping = false;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(HopAround());
    }

    private IEnumerator HopAround()
    {
        while (true)
        {
            if (!isHopping)
            {
                Vector2 hopDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                rb.AddForce(hopDirection * hopForce, ForceMode2D.Impulse);
                isHopping = true;

                yield return new WaitForSeconds(1f);
                isHopping = false;
            }

            yield return null;
        }
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
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            SplitOrDestroy();
        }
    }

    private void SplitOrDestroy()
    {
        if (currentHealth > minHealthForSplit || smallerSlimePrefab == null)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject smallerSlime = Instantiate(smallerSlimePrefab, transform.position, Quaternion.identity);
                EnemySlime slimeScript = smallerSlime.GetComponent<EnemySlime>();

                if (slimeScript != null)
                {
                    slimeScript.maxHealth = currentHealth / 2;
                    slimeScript.hopForce *= 0.75f;
                }
            }
        }

        Destroy(gameObject);
    }
}
