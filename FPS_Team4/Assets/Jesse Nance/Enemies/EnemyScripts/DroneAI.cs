using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    public Light spotlight;
    public GameObject player;
    public bool PlayerIsDetected {  get; private set; }
    public bool isCoolingDown;

    public float faceTargetSpeed;
    public float detectionCooldown;
    public float cooldownDelay;

    public delegate void PlayerDetectedHandler(Vector3 dronePosition);
    public event PlayerDetectedHandler OnPlayerDetected;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        PlayerIsDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();

        if(PlayerIsDetected && !isCoolingDown)
        {
            FaceTarget();
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

        StartCoroutine(DelayCooldown());
    }

    IEnumerator DelayCooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);

        isCoolingDown = true;
        yield return new WaitForSeconds(detectionCooldown);
        isCoolingDown = false;
    }
}
