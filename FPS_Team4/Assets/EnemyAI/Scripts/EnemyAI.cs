using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour, iDamage
{
    [Header("----- Components -----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Renderer modelBody;
    [SerializeField] Renderer modelArm;
    [SerializeField] Transform headPos;
    [SerializeField] Renderer model;

    [Header("----- Movement -----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] [Range(5, 20)] float roamingRadius = 20f; // Roaming Radius for random movement
    [SerializeField] bool isTurret = false; // Flag to determine if enemy is turret
    public int currentWayPointIndex;

    //Coroutine co;



    Vector3 playerDir;
    //Vector3 startingPosition;
    float angleToPlayer;

    Color colorBodyOrigin;
    Color colorArmOrigin;
    Color colorOrigin;

    [Header("----- Weapon Stats -----")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float stunTimer;
    [SerializeField] float hurtTimer;

    [Header("----- Bools -----")]
    bool playerVisible;
    bool playerInRange;
    bool isShooting;
    bool isStunned;
    bool isRoaming;

    // Start is called before the first frame update
    void Start()
    {
        colorBodyOrigin = modelBody.material.color;
        colorArmOrigin = modelBody.material.color;
        colorOrigin = model.material.color;

        GameManager.instance.RegisterThreat(); // Notify GameManager that enemy is active
    }

    // Update is called once per frame
    void Update()
    {

        if (!isStunned)
        {
            CanSeePlayer();

            if (playerVisible)
            {
                HandlePlayerVisible(); // call HandlePlayerVisible when the player is visible
            }
            else if (!playerInRange && !isTurret && waypoints.Count > 0)
            {
                HandlePlayerNotVisible();
            }
            else if (waypoints.Count == 0 && !isTurret)
            {
                RoamRandomly();
            }
        }

        //if(!isStunned && !playerInRange && !CanSeePlayer())
        //{
        //    if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        //    {
        //        currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
        //        MoveToNextWaypoint();
        //    }
        //}
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Count > 0)
        {
            navAgent.SetDestination(waypoints[currentWayPointIndex].position);
        }        
    }

    public void MoveToPosition(Vector3 position)
    {

        //if(navAgent != null)
        //{
        //    navAgent.SetDestination(position);
        //}
    }

    void CanSeePlayer()
    {
        if (isStunned || !playerInRange)
        {
            playerVisible = false;
            return;
        }

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            playerVisible = hit.collider.CompareTag("Player") && angleToPlayer <= FOV;
        }
        else
        {
            playerVisible = false;
        }
        //if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
        //{
        //    playerVisible = true;
        //    return;
        //}
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
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
            StopAllCoroutines();
            StartCoroutine(SearchForPlayer());
        }
    }

    IEnumerator SearchForPlayer()
    {
        float searchDuration = 5f; // Amount of time the enemy will be searching.
        float searchTimer = 0f;
        float searchRadius = 10f;

        while (searchTimer < searchDuration)
        {
            searchTimer += Time.deltaTime;
            CanSeePlayer();
            if (playerVisible)
            {
                yield break;
            }

            Vector3 randomDirection = Random.insideUnitCircle * searchRadius * transform.position;
            //randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, 1))
            {
                //Vector3 finalPosition = hit.position;
                navAgent.SetDestination(hit.position);
            }

            yield return new WaitForSeconds(1f);
        }

        if (!playerVisible)
        {
            currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
            MoveToNextWaypoint();
        }
    }

    //Taking Damage Functions
    public void takeDamage(int amount)
    {
        //For Damager
        HP -= amount;
        StartCoroutine(TurnColor(Color.yellow, hurtTimer));

        if (HP <= 0)
        {
            //I'm dead
            Destroy(gameObject);
        }
    }

    public void takeEMP(int amount)
    {
        StartCoroutine(Stun(stunTimer));
        StartCoroutine(TurnColor(Color.blue, stunTimer));
    }

    IEnumerator TurnColor(Color color, float duration)
    {
        modelBody.material.color = color;
        modelArm.material.color = color;
        yield return new WaitForSeconds(duration);
        modelBody.material.color = colorBodyOrigin;
        modelArm.material.color = colorArmOrigin;
    }

    //IEnumerator TurnYellow()
    //{
    //    modelBody.material.color = Color.yellow;
    //    modelArm.material.color = Color.yellow;
    //    yield return new WaitForSeconds(hurtTimer);
    //    modelBody.material.color = colorBodyOrigin;
    //    modelArm.material.color = colorArmOrigin;
    //}

    ////Flashes the Enemy Blue when EMP hits them.
    //IEnumerator TurnBlue()
    //{
    //    modelBody.material.color = Color.blue;
    //    modelArm.material.color = Color.blue;
    //    yield return new WaitForSeconds(stunTimer);
    //    modelBody.material.color = colorBodyOrigin;
    //    modelArm.material.color = colorArmOrigin;
    //}

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator Stun(float duration)
    {
        isStunned = true;
        if(!isTurret)
        {
            navAgent.isStopped = true;
        }        
        isShooting = false;

        GameManager.instance.OnStunBegin(); // Notify GameManager of stun

        yield return new WaitForSeconds(duration);

        if (!isTurret)
        {
            navAgent.isStopped = false;
        }
        isStunned = false;

        GameManager.instance.OnStunEnd(); // Notify GameManager of stun end
    }

    void HandlePlayerVisible()
    {
        if(!isTurret)
        {
            if (isRoaming)
            {
                // stop roaming if player is visible
                StopCoroutine(RoamCoroutine());
                isRoaming = false;
            }

            navAgent.SetDestination(GameManager.instance.player.transform.position);
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                FaceTarget();
            }
        }
        else
        {
            FaceTarget(); // Turret should aim at the player but not move
        }

        if (!isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    void HandlePlayerNotVisible()
    {
        if (waypoints.Count > 0 && !navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            if(isRoaming)
            {
                StopCoroutine(RoamCoroutine());
                isRoaming = false;
            }

            currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
            MoveToNextWaypoint();
        }
        else if(waypoints.Count == 0)
        {
            RoamRandomly();
        }
    }    

    void RoamRandomly()
    {
        if (!isRoaming)
        {
            isRoaming = true;
            StartCoroutine(RoamCoroutine());
        }
    }

    IEnumerator RoamCoroutine()
    {
        while (isRoaming) // Continue roaming only if isRoaming is true
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
    }
}
