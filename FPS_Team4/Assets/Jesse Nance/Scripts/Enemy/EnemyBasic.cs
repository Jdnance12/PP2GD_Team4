using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBasic : MonoBehaviour, IDamageable, IDisrupt
{
    [Header("---- Bools ----")]
    public bool isRoaming;
    public bool isShooting;
    public bool playerVisible;
    public bool playerInRange;
    public bool isDisrupted;

    [Header("---- Enemy Stats ----")]
    [SerializeField] public float HP;

    [Header("---- Enemy Movement ----")]
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamTimer;
    Coroutine coroutine;
    float angleToPlayer;

    float origStoppingDist;

    [SerializeField][Range(5, 20)] float roamingRadius = 20f;

    Vector3 playerDirection;
    Vector3 startingPosition;

    [Header("---- Enemy Attack ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;

    [Header("---- Enemy Disruption ----")]
    [SerializeField] float disruptionDuration;
    private Color disruptedColor = Color.blue;

    [Header("---- Enemy Components ----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer model;
    [SerializeField] GameObject player;
    [SerializeField] GameObject partsPrefab;

    Color origColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;

        origStoppingDist = navAgent.stoppingDistance;

        startingPosition = transform.position;

        origColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Roam());
    }


    //Movement Functions
    IEnumerator Roam()
    {
        if(navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;

            if(NavMesh.SamplePosition(randomDirection, out navHit, roamingRadius, -1))
            {
                navAgent.SetDestination(navHit.position);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        yield return null;
    }


    // Damage Functions
    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Instantiate(partsPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }
    public void causeDisrupt()
    {
        StartCoroutine(DisruptedRoutine());
    }
    IEnumerator DisruptedRoutine()
    {
        isDisrupted = true;
        model.material.color = disruptedColor;

        navAgent.isStopped = true;
        isShooting = false;

        yield return new WaitForSeconds(disruptionDuration);

        navAgent.isStopped = false;
        model.material.color = origColor;
        isDisrupted = false;
    }
}
