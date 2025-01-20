using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDrone : MonoBehaviour, IDamageable, IDisrupt
{
    private GameManager gm;

    [Header("---- Bools ----")]
    public bool isDisrupted;
    public bool isExploding;
    public bool isChasingPlayer;

    [Header("---- Stats ----")]
    [SerializeField] private float HP = 50f;
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackInterval = 1.5f;

    [Header("--- Movement Stats ----")]
    [SerializeField] private int faceTargetSpeed = 5;
    [SerializeField] private float roamRadius = 10f;

    [Header("---- Detection Settings ----")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private LayerMask detectionMask;

    private Coroutine roamCoroutine;
    private bool canAttack = true;

    [Header("---- Components ----")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform headPos;
    [SerializeField] private Renderer model;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private NavMeshAgent navAgent;

    private Color origColor;

    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;

        origColor = model.material.color;
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDisrupted || isExploding) return;

        if (DetectPlayer())
        {
            OnPlayerDetected();
        }
        else if (isChasingPlayer && !DetectPlayer())
        {
            OnPlayerLost();
        }
        else if (navAgent.remainingDistance < 0.1f && roamCoroutine == null)
        {
            roamCoroutine = StartCoroutine(Roaming());
        }
    }

    private bool DetectPlayer()
    {
        if (!player) return false;

        // Calculate direction and distance to the player
        Vector3 directionToPlayer = (player.transform.position - headPos.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.transform.position, headPos.position);

        // Check distance and field of view
        if (distanceToPlayer > detectionRange) return false;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        if (angleToPlayer > fieldOfViewAngle / 2) return false;

        // Check for line of sight
        if (Physics.Raycast(headPos.position, directionToPlayer, out RaycastHit hit, detectionRange, detectionMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Player detected
            }
        }
        return false; // Player not detected
    }

    private void OnPlayerDetected()
    {
        if (!isChasingPlayer)
        {
            isChasingPlayer = true;
            navAgent.isStopped = false;
            Debug.Log("Player detected! Chasing...");
            AudioManager.instance.PlayEnemyDetectedMusic();
        }
    }

    private void OnPlayerLost()
    {
        isChasingPlayer = false;
        Debug.Log("Lost sight of player. Returning to roam.");
        AudioManager.instance.PlayBackgroundMusic();
        roamCoroutine = StartCoroutine(Roaming());
    }

    private void ChasePlayer()
    {
        if (isChasingPlayer)
        {
            navAgent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.transform.position) <= navAgent.stoppingDistance)
            {
                AttackPlayer();
            }
        }
    }

    private void AttackPlayer()
    {
        if (canAttack && player.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(attackDamage);
            Debug.Log("EnemyDrone attacked the player!");
            canAttack = false;
            Invoke(nameof(ResetAttack), attackInterval);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    IEnumerator Roaming()
    {
        Vector3 randomPOS = Random.insideUnitSphere * roamRadius + transform.position;

        if (NavMesh.SamplePosition(randomPOS, out NavMeshHit navHit, roamRadius, NavMesh.AllAreas))
        {
            navAgent.SetDestination(navHit.position);
        }

        yield return new WaitForSeconds(Random.Range(3, 8));
        roamCoroutine = null;
    }

    IEnumerator Explode()
    {
        isExploding = true;
        navAgent.isStopped = true;

        model.material.color = Color.red;
        yield return new WaitForSeconds(explosionDelay);

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player") && hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            StartCoroutine(Explode());
        }
    }

    public void causeDisrupt()
    {
        StartCoroutine(Disrupted());
    }

    IEnumerator Disrupted()
    {
        isDisrupted = true;
        model.material.color = Color.blue;

        navAgent.isStopped = true;
        yield return new WaitForSeconds(3);

        navAgent.isStopped = false;
        model.material.color = origColor;
        isDisrupted = false;
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
