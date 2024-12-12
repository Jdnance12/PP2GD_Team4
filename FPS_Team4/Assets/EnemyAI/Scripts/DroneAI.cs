using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    public Light spotlight;
    public GameObject lightPos;
    public GameObject player;
    public Renderer model;
    public NavMeshAgent agent;

    [SerializeField] int faceTargetSpeed;

    Vector3 playerDir;
    float angleToPlayer;

    [SerializeField] float stunTimer;

    public bool isStunned;
    public bool playerIsDetected;

    Color colorOrigin;
    Color spotlightOriginColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;

        colorOrigin = model.material.color;
        spotlightOriginColor = spotlight.color;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsDetected)
        {
            DroneMovement();
        }
        else
        {
            spotlight.color = spotlightOriginColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsDetected = true;
            spotlight.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsDetected = false;
            spotlight.color = spotlightOriginColor;
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
}
