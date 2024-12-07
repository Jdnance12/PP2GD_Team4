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

    public bool PlayerIsDetected {  get; private set; }
    public bool isCoolingDown;
    bool isStunned; // Stun for actie threat count

    //Vector3 playerLastKnownLocation;

    public int HP;
    public float faceTargetSpeed;
    public float detectionCooldown;
    public float cooldownDelay;
    public float stunTimer;

    public delegate void PlayerDetectedHandler(Vector3 dronePosition);
    public event PlayerDetectedHandler OnPlayerDetected;

    Color colorOrigin;

    // Start is called before the first frame update
    void Start()
    {
        colorOrigin = model.material.color;

        player = GameManager.instance.player;
        PlayerIsDetected = false;

        GameManager.instance.RegisterThreat(); // Notify GameManager that drone is active
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned == false)
        {
            DetectPlayer();
        }
        

        if(isStunned == false && PlayerIsDetected)
        {
            FaceTarget();
            CreateInspectionPoint();
        }
    }

    void DetectPlayer()
    {
       if(player == null)
       {
            return;
       }

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angle = Vector3.Angle(spotlight.transform.forward, directionToPlayer);

        if(distanceToPlayer < spotlight.range && angle < spotlight.spotAngle / 2f)
        {
            RaycastHit hit;
            if(Physics.Raycast(spotlight.transform.position, directionToPlayer, out hit))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    PlayerIsDetected = true;
                    //playerLastKnownLocation = player.transform.position;
                    OnPlayerDetected?.Invoke(transform.position);

                    return;
                }
            }
        }
        PlayerIsDetected = false;
    }

    void FaceTarget()
    {
        Vector3 playerDir = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

        
    }

    IEnumerator DelayCooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);

        isCoolingDown = true;
        yield return new WaitForSeconds(detectionCooldown);
        isCoolingDown = false;
    }

    void CreateInspectionPoint()
    {
        if(GameManager.instance.inspectionPointPrefab != null)
        {
            //GameObject inspectionPoint = Instantiate(GameManager.instance.inspectionPointPrefab, playerLastKnownLocation, Quaternion.identity);
        }
    }

    void NotifyNearbyEnemies(Vector3 inspectionPointPos)
    {
        float radius = 15f;
        Collider[] colliders = Physics.OverlapSphere(inspectionPointPos, radius);
        foreach (Collider collider in colliders)
        {
            EnemyAI enemyAI = collider.GetComponent<EnemyAI>();
            if(enemyAI != null)
            {
                enemyAI.OnPlayerDetected(inspectionPointPos);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(TurnBlue());

        StartCoroutine(Stun(stunTimer));
    }

    IEnumerator TurnBlue()
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(stunTimer);
        model.material.color = colorOrigin;
    }

    public IEnumerator Stun(float stunDuration)
    {
        isStunned = true; // Mark as stunned
        PlayerIsDetected = false; // Disable detection
        lightPos.SetActive(false);
        GameManager.instance.OnStunBegin(); // Notify GameManager of stun

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; // Recover from stun
        lightPos.SetActive(true);
        GameManager.instance.OnStunEnd(); // Notify GameManager of stun end
    }

}
