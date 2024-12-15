using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FirstEnemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer modelBody;
    [SerializeField] Renderer modelArm;
    [SerializeField] NavMeshAgent navAgent;

    public TextMeshProUGUI uiTitleText;
    public TextMeshProUGUI uiBodyText;

    [SerializeField] int HP;
    [SerializeField] float hurtTimer;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform headPos;
    Vector3 playerDir;
    Vector3 startingPosition;
    float angleToPlayer;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorBodyOrigin;
    Color colorArmOrigin;

    bool playerInRange;
    bool playerVisible;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        colorBodyOrigin = modelBody.material.color;
        colorArmOrigin = modelBody.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();

        if (playerVisible)
        {
            navAgent.SetDestination(GameManager.instance.player.transform.position);
            if (navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                FaceTarget();
            }
            if (!isShooting)
            {
                StartCoroutine(shoot());
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

    void CanSeePlayer()
    {
        if(playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            Debug.DrawRay(transform.position, playerDir);

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
                {
                    playerVisible = true;              
                }
                else
                {
                    playerVisible = false;
                }
            }
        }
        
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(TurnYellow());

        if (HP <= 0)
        {
            GameManager.instance.DialogueScreen();
            uiTitleText.text = "Enemy was Defeated!";
            uiBodyText.text = "Now that the enemy is defeated. Let's go find the nearby heal station. Behind you is a green door. The heal station is beyond it.";

            //I'm dead
            Destroy(gameObject);
        }
    }
    IEnumerator TurnYellow()
    {
        modelBody.material.color = Color.yellow;
        modelArm.material.color = Color.yellow;
        yield return new WaitForSeconds(hurtTimer);
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
}
