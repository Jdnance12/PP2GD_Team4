using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoss : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Attack Settings")]
    [SerializeField] private float attackInterval = 3.0f;
    [SerializeField] private float attackRange = 5.0f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Phase Settings")]
    [SerializeField] private int phaseTwoThreshold = 50;
    private bool isPhaseTwo = false;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;

    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        CheckPhase();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(1.0f);

        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        }
        else if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(attackInterval);

        isAttacking = false;
    }

    private void CheckPhase()
    {
        if (!isPhaseTwo && currentHealth <= phaseTwoThreshold)
        {
            isPhaseTwo = true;
            EnterPhaseTwo();
        }
    }

    private void EnterPhaseTwo()
    {
        attackInterval *= 0.75f;

        if (animator != null)
        {
            animator.SetTrigger("PhaseTwo");
        }

        Debug.Log("Boss has entered Phase Two!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        GetComponent<Collider>().enabled = false;
        this.enabled = false;

    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
