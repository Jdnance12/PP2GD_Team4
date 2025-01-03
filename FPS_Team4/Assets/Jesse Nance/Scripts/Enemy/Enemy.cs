using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private GameManager gm;

    [Header("---- Bools ----")]
    public bool isDisrupted;
    public bool playerInRange;
    
    [Header("---- Stats ----")]
    [SerializeField] float HP;

    [Header("--- Movement Stats ----")]
    [SerializeField] int faceTargetSpeed;

    [Header("---- Components ----")]
    [SerializeField] GameObject player;
    [SerializeField] Renderer model;
    [SerializeField] GameObject partsPrefab;
    [SerializeField] NavMeshAgent navAgent;

    private Vector3 playerDir;

    Color origColor;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;

        origColor = model.material.color; // To get the original color so flash red will revert back to it.
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDir = player.transform.position - transform.position;

            navAgent.SetDestination(GameManager.instance.player.transform.position);

            if(navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                FaceTarget();
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

    // Player/Object Detection
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed()); // Call flash red when damage is taken

        if (HP <= 0)
        {
            Instantiate(partsPrefab, transform.position, Quaternion.identity); // Drops the parts currency when the enemy is destoryed
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f); //Turns red for 1 second
        model.material.color = origColor;
    }
}
