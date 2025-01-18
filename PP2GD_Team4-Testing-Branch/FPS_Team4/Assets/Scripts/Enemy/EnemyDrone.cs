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
    [SerializeField] float HP = 50f;
    [SerializeField] float explosionRange = 5f;
    [SerializeField] float explosionDamage = 50f;
    [SerializeField] float explosionDelay = 1f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackInterval = 1.5f; 

    [Header("--- Movement Stats ----")]
    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] float roamRadius = 10f;

    private Coroutine roamCoroutine;
    private bool canAttack = true;

    [Header("---- Components ----")]
    [SerializeField] GameObject player;
    [SerializeField] Transform headPos;
    [SerializeField] Renderer model;
    [SerializeField] GameObject explosionEffectPrefab;
    [SerializeField] NavMeshAgent navAgent;

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= explosionRange)
        {
            StartCoroutine(Explode());
        }
        else if (isChasingPlayer)
        {
            navAgent.SetDestination(player.transform.position);

            if (distanceToPlayer <= navAgent.stoppingDistance)
            {
                AttackPlayer();
            }
        }
        else if (navAgent.remainingDistance < 0.1f && roamCoroutine == null)
        {
            roamCoroutine = StartCoroutine(Roaming());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Explode());
        }
    }

    public void OnHackingPuzzleResult(bool success)
    {
        if (!success)
        {
            EngagePlayer();
        }
    }

    private void EngagePlayer()
    {
        isChasingPlayer = true;
        navAgent.isStopped = false;
        Debug.Log("EnemyDrone is now chasing the player!");
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
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Vector3 randDirection = Random.insideUnitSphere * roamRadius;
            randDirection += transform.position;

            if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, roamRadius, NavMesh.AllAreas))
            {
                navAgent.SetDestination(navHit.position);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
            roamCoroutine = null;
        }
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
            if (hit.CompareTag("Player"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(explosionDamage);
                }
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
    }
}
