using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    public GameObject player;
    public Renderer model;
    public NavMeshAgent agent;

    [SerializeField] int faceTargetSpeed;
    [SerializeField] PlayerDetection playerDetection;

    Vector3 playerDir;
    float angleToPlayer;

    [SerializeField] float stunTimer;

    public bool isStunned;
    public bool playerIsDetected;

    Color colorOrigin;

    public List<GameObject> detectedEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;

        colorOrigin = model.material.color;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerIsDetected = playerDetection.playerIsDetected;

        if (playerIsDetected)
        {
            DroneMovement();
        }
    }

    bool DroneMovement()
    {
        if (isStunned) return false;

        if (playerIsDetected)
        {
            playerDir = player.transform.position - transform.position;
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            Debug.DrawRay(transform.position, playerDir);

            FaceTarget();
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            return true;
        }
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void AlertEnemies()
    {
        foreach (GameObject enemy in detectedEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if ((enemyAI != null))
            {
                enemyAI.MoveToPosition(transform.position);
            }
        }
    }
    IEnumerator TurnBlue()
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(stunTimer);
        model.material.color = colorOrigin;
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;

        GameManager.instance.OnStunBegin(); // Notify GameManager of stun

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;

        GameManager.instance.OnStunEnd(); // Notify GameManager of stun end
    }
    public void takeDamage(int amount)
    {
        //HP -= amount;
        StartCoroutine(TurnBlue());

        StartCoroutine(Stun(stunTimer));
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            detectedEnemies.Add(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            detectedEnemies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        detectedEnemies.Remove(other.gameObject);
    }
}
