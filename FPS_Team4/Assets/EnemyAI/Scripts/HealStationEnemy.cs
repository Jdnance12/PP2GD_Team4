using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HealStationEnemy : MonoBehaviour, iDamage
{
    [SerializeField] Renderer modelBody;
    [SerializeField] Renderer modelArm;
    [SerializeField] NavMeshAgent navAgent;

    [SerializeField] int HP;
    [SerializeField] float hurtTimer;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform headPos;

    Vector3 playerDir;
    float angleToPlayer;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorBodyOrigin;
    Color colorArmOrigin;

    bool playerInRange;
    bool playerVisible;
    bool isShooting;

    [SerializeField] GameObject itemPrefab;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image background;

    Vector3 dropPos;

    void Start()
    {
        Debug.Log("HealStationEnemy initialized.");
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
                StartCoroutine(Shoot());
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
                    Debug.Log("Player not visible.");
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

        dropPos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        Debug.Log($"HealStationEnemy took {amount} damage. Current HP: {HP}");
        HP -= amount;

        StartCoroutine(TurnYellow());

        if (HP <= 0)
        {
            GameManager.instance.statePause();
            GameManager.instance.DialogueScreen();

            background.color = Color.green;
            titleText.text = "Skills";
            bodyText.text = "Look he dropped something. Grab it. It could be useful!";

            Instantiate(itemPrefab, dropPos, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    IEnumerator TurnYellow()
    {
        Debug.Log("Enemy hit: turning yellow");
        modelBody.material.color = Color.yellow;
        modelArm.material.color = Color.yellow;

        yield return new WaitForSeconds(hurtTimer);

        modelBody.material.color = colorBodyOrigin;
        modelArm.material.color = colorArmOrigin;
        Debug.Log("Returning to original color.");
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Debug.Log("Shooting bullet");
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        Debug.Log("Finished shooting. Ready to shoot again.");
    }

    void DropItem()
    {
        Debug.Log("Dropping item");
        Vector3 dropPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Instantiate(itemPrefab, dropPos, Quaternion.identity);
        Debug.Log($"Item dropped at position: {dropPos}");
    }
}