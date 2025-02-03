using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBasic : MonoBehaviour, IDamageable, IDisrupt
{
    private GameManager gm;

    public event System.Action<EnemyBasic> OnDeath;

    [Header("---- Bools ----")]
    public bool isDisrupted;
    public bool isShooting;
    public bool playerInRange;
   

    [Header("---- Stats ----")]
    [SerializeField] public float HP;
    [SerializeField] float shootRate;

    [SerializeField] public bool bossModifierApplied; // track if boss modifier is applied
    [SerializeField] public bool spawnerModifierApplied; // track if spawner modifier is applied

    [Header("--- Movement Stats ----")]
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float angleToPlayer;
    [SerializeField] int disruptDuration;
    [SerializeField] float roamRadius;
    [SerializeField] float stopDistance = 10.0f;
    [SerializeField] float sidestepDistance = 1.5f;
    [SerializeField] float sidestepRate = 1.0f;

    private Coroutine co;
    private Vector3 lastKnownPosition;
    private bool isChasingLastKnownPosition;

    [Header("---- Components ----")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] Renderer model;
    [SerializeField] GameObject partsPrefab;
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject damageTextPos;
    [SerializeField] NavMeshAgent navAgent;

    [Header("---- Audio ----")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] laserSounds;

    public float shootDamage; // this holds the modified value for damage

    private Vector3 playerDir;

    Color origColor;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;

        origColor = model.material.color; // To get the original color so flash red will revert back to it.
        navAgent = GetComponent<NavMeshAgent>();

        //// Debug logs to confirm initialization
        //Debug.Log(gameObject.name + ": AudioSource assigned: " + (audioSource != null));
        //Debug.Log(gameObject.name + ": Player assigned: " + (player != null));
        //Debug.Log(gameObject.name + ": Bullet assigned: " + (bullet != null));
        //Debug.Log(gameObject.name + ": Shoot position assigned: " + (shootPos != null));
        //Debug.Log(gameObject.name + ": Laser sounds array length: " + laserSounds.Length);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDisrupted)
        {
            if (CanSeePlayer())
            {
                // update last known position
                lastKnownPosition = player.transform.position;

                // stop chasing last known
                isChasingLastKnownPosition = false;

                // enemy can see player. start shooting
                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }
            }
            else if (isChasingLastKnownPosition)
            {
                // player is not in range or cant be seen
                if (navAgent.remainingDistance < 0.1f)
                {
                    StartCoroutine(Roaming());
                }
            }
            else
            {
                if (navAgent.remainingDistance < 0.1f)
                {
                    StartCoroutine(Roaming());
                }
            }
        }
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
        }
    }

    // Player/Object Detection and Movement
    void FaceTarget(Vector3 target)
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(target.x, 0, target.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Roaming()
    {
        if(navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            Vector3 randDirection = Random.insideUnitSphere * roamRadius;
            randDirection += transform.position;

            NavMeshHit navHit;
            if(NavMesh.SamplePosition(randDirection, out navHit, roamRadius, -1))
            {
                navAgent.SetDestination(navHit.position);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        yield return null;
    }

    bool CanSeePlayer()
    {
        playerDir = player.transform.position + Vector3.up * 2.0f - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                float bufferRange = 1.0f;
                //Debug.Log("The distance to player is :" + distanceToPlayer + ". Stop Distance: " + stopDistance);
                if (distanceToPlayer > stopDistance + bufferRange)
                {
                    Vector3 moveToPosition = player.transform.position + (transform.position - player.transform.position).normalized * stopDistance;

                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(moveToPosition, out navHit, stopDistance, NavMesh.AllAreas))
                    {
                        navAgent.SetDestination(navHit.position);
                        //Debug.Log("Moving towards player, Position: " + navHit.position);
                    }
                    FaceTarget(playerDir);
                }
                else if (distanceToPlayer < stopDistance - bufferRange)
                {
                    Vector3 moveAwayDir = (transform.position - player.transform.position).normalized;
                    Vector3 moveAwayTarget = player.transform.position + moveAwayDir * stopDistance;

                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(moveAwayTarget, out navHit, stopDistance, NavMesh.AllAreas))
                    {
                        navAgent.SetDestination(navHit.position);
                        //Debug.Log("Moving away from Player, Position: " + navHit.position);
                    }
                    FaceTarget(playerDir);
                }
                else
                {
                    FaceTarget(playerDir);
                }
                return true;
            }
        }
        return false;
    }


    //Enemy Shooting
    IEnumerator Shoot()
    {
        isShooting = true;

        // sidestep logic
        Vector3 sidestepDir = Vector3.Cross(Vector3.up, playerDir).normalized * sidestepDistance;
        if (Random.value > 0.5f) // random move left or right
        {
            sidestepDir = -sidestepDir;
        }
        Vector3 sidestepTarget = transform.position + sidestepDir;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(sidestepTarget, out navHit, sidestepDistance, -1))
        {
            navAgent.SetDestination(sidestepTarget);

            yield return new WaitForSeconds(0.1f);

            FaceTarget(player.transform.position);
        }

        // calc direction towards center of mass
        Vector3 bulletDirection = (player.transform.position + Vector3.up * 0.5f - shootPos.position).normalized;

        GameObject newBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(bulletDirection));
        Debug.Log(gameObject.name + ": Bullet instantiated at position: " + shootPos.position);
        Bullet bulletComponent = newBullet.GetComponent<Bullet>();
        if(bulletComponent != null )
        {
            bulletComponent.SetDamage(shootDamage);
            bulletComponent.SetIsEnemyBullet(true); // Mark bullet as an enemy bullet so it ignores other enemies
            Debug.Log(gameObject.name + ": Bullet damage set to: " + bulletComponent.damage);
        }

        if (audioSource != null && laserSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, laserSounds.Length);
            audioSource.PlayOneShot(laserSounds[randomIndex]);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }


    // Damage and Disruption
    public void causeDisrupt()
    {
        StartCoroutine(Disrupted());
    }  
    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed()); // Call flash red when damage is taken

        if (HP <= 0)
        {
            OnDeath?.Invoke(this); // trigger OnDeath event
            Instantiate(partsPrefab, transform.position, Quaternion.identity); // Drops the parts currency when the enemy is destoryed
            Destroy(gameObject);
        }
    }
    IEnumerator Disrupted()
    {
        isDisrupted = true;
        model.material.color = Color.blue;

        navAgent.isStopped = true;
        isShooting = false;

        yield return new WaitForSeconds(disruptDuration);

        navAgent.isStopped = false;
        model.material.color = origColor;
        isDisrupted = false;
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.1f); //Turns red for 1 second
        model.material.color = origColor;
    }

    public void ApplyModifiers(float hpModifier, float damageModifier, float bossHpModifier, float bossDamageModifier)
    {
        HP *= (hpModifier * bossHpModifier); // apply modifier to existing hp
        shootDamage = (damageModifier * bossDamageModifier); // apply modifier to existing shootDamage
    }


}
