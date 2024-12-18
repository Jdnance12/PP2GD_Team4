using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretAI : MonoBehaviour, iDamage
{
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform headPos;
    [SerializeField] Renderer model;

    Vector3 playerDir;
    float angleToPlayer;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float stunTimer;

    bool playerInRange;
    bool isShooting;
    bool isStunned;

    Color colorOrigin;

    // Start is called before the first frame update
    void Start()
    {
        colorOrigin = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if ((!isStunned && playerInRange && CanSeePlayer()))
        {

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

    bool CanSeePlayer()
    {
        if (isStunned) return false;

        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                FaceTarget();
                
                if (!isShooting)
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

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
        isShooting = false;

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
