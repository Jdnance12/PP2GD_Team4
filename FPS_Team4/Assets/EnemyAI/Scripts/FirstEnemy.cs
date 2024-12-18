using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FirstEnemy : MonoBehaviour, iDamage
{
    [SerializeField] Renderer modelBody;
    [SerializeField] Renderer modelArm;
    [SerializeField] NavMeshAgent navAgent;

    [SerializeField] int HP;
    [SerializeField] float hurtTimer;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float stunTimer;
    [SerializeField] Transform headPos;

    Vector3 playerDir;
    float angleToPlayer;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorBodyOrigin;
    Color colorArmOrigin;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image background;

    bool playerInRange;
    bool playerVisible;
    bool isShooting;
    bool isStunned;

    void Start()
    {
        Debug.Log("FirstEnemy initialized.");
        colorBodyOrigin = modelBody.material.color;
        colorArmOrigin = modelArm.material.color;
    }

    void Update()
    {
        CanSeePlayer();

        if (playerVisible)
        {
            navAgent.SetDestination(GameManager.instance.player.transform.position);
            Debug.Log("Navigating to player...");

            if (navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                Debug.Log("Player is in range. Facing target...");
                FaceTarget();
            }

            if (!isShooting)
            {
                Debug.Log("Starting shooting coroutine...");
                StartCoroutine(shoot());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered detection range.");
            playerInRange = true;
        }
    }

    void CanSeePlayer()
    {
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(playerDir, transform.forward);

            Debug.DrawRay(headPos.position, playerDir, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
                {
                    Debug.Log("Player is visible.");
                    playerVisible = true;
                }
                else
                {
                    playerVisible = false;
                }
            }
            else
            {
                Debug.Log("Raycast did not hit the player.");
                playerVisible = false;
            }
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        Debug.Log("Rotating to face player.");
    }

    public void takeDamage(int amount)
    {
        Debug.Log($"Enemy took {amount} damage. Current HP: {HP}");
        HP -= amount;

        StartCoroutine(TurnYellow());

        if (HP <= 0)
        {
            GameManager.instance.statePause();
            GameManager.instance.DialogueScreen();

            background.color = Color.blue;
            titleText.text = "Enemy Defeated!";
            bodyText.text = "You've defeated your first enemy" +
                "Now to heal. Look for the green door in this room." +
                "Beyond it you will find a heal station.";

            Destroy(gameObject);  
        }
    }

    public void takeEMP(int amount)
    {
        StartCoroutine(Stun(stunTimer));
        StartCoroutine(TurnBlue());
    }

    IEnumerator TurnYellow()
    {
        Debug.Log("Enemy hit: turning yellow...");
        modelBody.material.color = Color.yellow;
        modelArm.material.color = Color.yellow;

        yield return new WaitForSeconds(hurtTimer);

        modelBody.material.color = colorBodyOrigin;
        modelArm.material.color = colorArmOrigin;
        Debug.Log("Returning to original color.");
    }

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
        Debug.Log("Shooting bullet...");
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        Debug.Log("Finished shooting. Ready to shoot again.");
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