using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;

    Color colorOrigin;

    Vector3 playerDir;

    float angleToPlayer;

    bool playerInRange;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        colorOrigin = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayerDetected(Vector3 targetPosition)
    {
        targetPosition = new Vector3(GameManager.instance.drone.transform.position.x, 0, GameManager.instance.drone.transform.position.y);
        navAgent.SetDestination(targetPosition);
    }

    bool CanSeePlayer()
    {
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
        StartCoroutine(FlashBlue());

        if (HP <= 0)
        {
            //I'm stunned
        }
    }

    //Flashes the Enemy Blue when EMP hits them.
    IEnumerator FlashBlue()
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(0.5f);
        model.material.color = colorOrigin;
    }
}
