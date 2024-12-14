using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour, IDamage
{
    //For Movement and Detection
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer modelBody;
    [SerializeField] Renderer modelArm;
    [SerializeField] Transform headPos;
    Vector3 playerDir;
    float angleToPlayer;

    //For shooting
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float stunTimer;

    Color colorBodyOrigin;
    Color colorArmOrigin;

    //Bools
    bool playerInRange;
    bool isShooting;
    bool isStunned;

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
        if ((!isStunned && playerInRange && CanSeePlayer()))
        {

        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if(navAgent != null)
        {
            navAgent.SetDestination(position);
        }
    }

    bool CanSeePlayer()
    {
        if (isStunned) return false;

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit))
        {
           if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
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
                return true;
            }
        }
        return false;
        
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
        }
    }

    //Taking Damage Functions
    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(TurnBlue());

        StartCoroutine(Stun(stunTimer));
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
