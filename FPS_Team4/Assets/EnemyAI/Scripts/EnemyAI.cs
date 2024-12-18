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

    [Header("----- Movement -----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    public int currentWayPointIndex;

    Coroutine co;

    
    
    Vector3 playerDir;
    Vector3 startingPosition;
    float angleToPlayer;

    Color colorBodyOrigin;
    Color colorArmOrigin;

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

        GameManager.instance.RegisterThreat(); // Notify GameManager that enemy is active
    }

    // Update is called once per frame
    void Update()
    {

        if(!isStunned && !isRoaming)
        {
            CanSeePlayer();

            if(playerVisible)
            {
                navAgent.SetDestination(GameManager.instance.player.transform.position);
                if(navAgent.remainingDistance < navAgent.stoppingDistance)
                {
                    FaceTarget();
                }
                if(!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }

            else
            {
                if (!playerInRange)
                {
                    if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
                        MoveToNextWaypoint();
                    }
                }               
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
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        navAgent.SetDestination(waypoints[currentWayPointIndex].position);
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
        if(isStunned || !playerInRange)
        {
            playerVisible = false;
            return;
        }

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                playerVisible = true;
                return;
            }
        }
        playerVisible = false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopCoroutine("SearchForPlayer");
            StartCoroutine("SearchForPlayer");            
        }
    }

    IEnumerator SearchForPlayer()
    {
        float searchDuration = 5f; // Amount of time the enemy will be searching.
        float searchTimer = 0f;
        float searchRadius = 10f;

        while(searchTimer< searchDuration)
        {
            searchTimer += Time.deltaTime;
            CanSeePlayer();
            if(playerVisible)
            {
                yield break;
            }

            Vector3 randomDirection = Random.insideUnitCircle * searchRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomDirection, out hit, searchRadius, 1))
            {
                Vector3 finalPosition = hit.position;
                navAgent.SetDestination(finalPosition);
            }
            
            yield return new WaitForSeconds(1f);
        }

        if(!playerVisible)
        {
            currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Count;
            MoveToNextWaypoint();
        }
    }

    //Taking Damage Functions
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(TurnYellow());

        if (HP <= 0)
        {
            //I'm dead
            Destroy(gameObject);
        }

        //StartCoroutine(Stun(stunTimer));
    }

    IEnumerator TurnYellow()
    {
        modelBody.material.color = Color.yellow;
        modelArm.material.color = Color.yellow;
        yield return new WaitForSeconds(hurtTimer);
        modelBody.material.color = colorBodyOrigin;
        modelArm.material.color = colorArmOrigin;
    }

    //Flashes the Enemy Blue when EMP hits them.
    IEnumerator TurnBlue()
    {
        modelBody.material.color = Color.blue;
        modelArm.material.color = Color.blue;
        yield return new WaitForSeconds(stunTimer);
        modelBody.material.color = colorBodyOrigin;
        modelArm.material.color = colorArmOrigin;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        navAgent.isStopped = true;
        isShooting = false;

        GameManager.instance.OnStunBegin(); // Notify GameManager of stun

        yield return new WaitForSeconds(stunDuration);

        navAgent.isStopped = false;
        isStunned = false;

        GameManager.instance.OnStunEnd(); // Notify GameManager of stun end
    }
}
