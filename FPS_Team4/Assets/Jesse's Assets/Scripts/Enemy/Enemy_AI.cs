using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour, IDamageable, IDisrupt
{
    [Header("---- Enemy Components ----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer model;
    [SerializeField] GameObject player;
    [SerializeField] GameObject partsPrefab;

    Color origColor;

    [Header("---- Enemy Attack ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;


    [Header("---- Enemy Stats ----")]
    [SerializeField] public int HP;

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

    [Header("---- Bools ----")]
    public bool isRoaming;
    public bool isShooting;
    public bool playerVisible;
    public bool playerInRange;
    public bool isDisrupted;


    // Start is called before the first frame update
    void Start()
    {
        player = gameManager.instance.player;

        origStoppingDist = navAgent.stoppingDistance;

        startingPosition = transform.position;

        origColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && !FollowPlayer())
        {
            if(!isRoaming && navAgent.remainingDistance < 0.01f)
            {
                coroutine = StartCoroutine(RoamCoroutine());
            }
        }
        else if (!playerInRange)
        {
            if(!isRoaming && navAgent.remainingDistance < 0.1f)
            {
                coroutine = StartCoroutine(RoamCoroutine());
            }
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator RoamCoroutine()
    {
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;

            if (NavMesh.SamplePosition(randomDirection, out navHit, roamingRadius, -1))
            {
                navAgent.SetDestination(navHit.position);
            }

            yield return new WaitForSeconds(Random.Range(3, 8)); // Wait a random time between 3 and 8 seconds before moving again
        }
        yield return null; // continue checking until the destination is reached
    }

    bool FollowPlayer()
    {
        playerDirection = player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        Debug.DrawRay(transform.position, playerDirection);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                navAgent.SetDestination(player.transform.position);

                if(navAgent.remainingDistance < navAgent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Instantiate(partsPrefab, transform.position, Quaternion.identity);
            Destroy();
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
        isDisrupted = true;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
