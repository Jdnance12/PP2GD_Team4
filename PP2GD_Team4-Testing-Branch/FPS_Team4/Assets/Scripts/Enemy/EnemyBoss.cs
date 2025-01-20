using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour, IDamageable
{
    [Header("---- Stats ----")]
    [SerializeField] private float HP;
    [SerializeField] private float phaseTwoThreshold = 0.7f; // 70% HP
    [SerializeField] private float phaseThreeThreshold = 0.4f; // 40% HP
    [SerializeField] private float attackCooldown = 2f;

    [Header("---- Components ----")]
    [SerializeField] private Renderer model;
    [SerializeField] private GameObject partsPrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] attackPoints;

    private Color origColor;
    private bool isBossActive;
    private bool canAttack = true;

    private int currentPhase = 1;

    void Start()
    {
        origColor = model.material.color;
    }

    void Update()
    {
        if (isBossActive)
        {
            HandleBossBehavior();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            EndBossFight();
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }

    public void StartBossFight()
    {
        isBossActive = true;
        Debug.Log("Boss fight started!");
    }

    private void EndBossFight()
    {
        Debug.Log("Boss defeated!");
        Instantiate(partsPrefab, transform.position, Quaternion.identity);
        Instantiate(nodePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void HandleBossBehavior()
    {
        if (!canAttack) return;

        // Determine phase based on remaining HP
        if (HP / phaseTwoThreshold < 1f && currentPhase == 1)
        {
            currentPhase = 2;
            Debug.Log("Boss entering Phase 2!");
        }
        else if (HP / phaseThreeThreshold < 1f && currentPhase == 2)
        {
            currentPhase = 3;
            Debug.Log("Boss entering Phase 3!");
        }

        // Execute phase-specific behavior
        switch (currentPhase)
        {
            case 1:
                StartCoroutine(PerformBasicAttack());
                break;

            case 2:
                StartCoroutine(PerformAdvancedAttack());
                break;

            case 3:
                StartCoroutine(PerformFinalAttack());
                break;
        }
    }

    IEnumerator PerformBasicAttack()
    {
        canAttack = false;
        Debug.Log("Boss performing basic attack!");

        foreach (Transform attackPoint in attackPoints)
        {
            Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator PerformAdvancedAttack()
    {
        canAttack = false;
        Debug.Log("Boss performing advanced attack!");

        for (int i = 0; i < attackPoints.Length; i++)
        {
            Instantiate(projectilePrefab, attackPoints[i].position, attackPoints[i].rotation);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator PerformFinalAttack()
    {
        canAttack = false;
        Debug.Log("Boss performing final attack!");

        for (int i = 0; i < attackPoints.Length; i++)
        {
            Instantiate(projectilePrefab, attackPoints[i].position, attackPoints[i].rotation);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < attackPoints.Length; i++)
        {
            Instantiate(projectilePrefab, attackPoints[i].position, attackPoints[i].rotation);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartBossFight();
        }
    }
}
