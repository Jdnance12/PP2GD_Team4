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

    // MK - ADDED START
    [SerializeField] private float stoppingDistance = 7.0f; // Distance at which enemy stops before reaching player
    // MK - ADDED END

    private Coroutine co;

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
            if (playerInRange && !CanSeePlayer())
            {
                if(navAgent.remainingDistance < 0.01f)
                {
                    StartCoroutine(Roaming());
                }
            }
            else if (!playerInRange)
            {
                if (navAgent.remainingDistance < 0.01f)
                {
                    StartCoroutine(Roaming());
                }
            }
            StartCoroutine(Roaming());
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
                navAgent.stoppingDistance = stoppingDistance; // MK - ADDED: Adjusts stopping distance dynamically
                navAgent.SetDestination(player.transform.position); // Moves toward player

                if (navAgent.remainingDistance <= stoppingDistance) // MK - CHANGED: Ensure enemy stops
                {
                    navAgent.isStopped = true; // MK - ADDED: Halts movement when within attack range
                    FaceTarget(playerDir);
                }
                else
                {
                    navAgent.isStopped = false; // MK - ADDED: Allows movement when not within stopping distance
                }

                if (!isShooting)
                {
                    StartCoroutine(Shoot());
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

        GameObject newBullet = Instantiate(bullet, shootPos.position, transform.rotation);
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

    // MK - ADDED START
    public void SetTarget(Vector3 playerPosition)
    {
        navAgent.SetDestination(playerPosition); // Forces enemy to chase player
        navAgent.stoppingDistance = stoppingDistance; // MK - ADDED: Enemy stops short of the player
    }
    // MK - ADDED END

    // Damage and Disruption
    public void causeDisrupt()
    {
        StartCoroutine(Disrupted());
    }  
    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed()); // Call flash red when damage is taken

        if (HP > 0) // MK - CHANGED: Alert enemies only if they are still alive
        {
            SetTarget(player.transform.position); // MK - ADDED: Enemy chases player when hit
        }
        else
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
