using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class HealStationEnemy : MonoBehaviour, IDamage
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
        Debug.Log($"HealStationEnemy took {amount} damage. Current HP: {HP}");
        HP -= amount;

        StartCoroutine(TurnYellow());

        if (HP <= 0)
        {
            Debug.Log("HealStationEnemy HP <= 0. Starting death sequence.");
            StartCoroutine(ShowDialogueAfterDeath());
        }
    }

    private IEnumerator ShowDialogueAfterDeath()
    {
        Debug.Log("ShowDialogueAfterDeath coroutine started.");
        yield return new WaitForSecondsRealtime(0.1f);

        Debug.Log("Attempting to display dialogue screen");

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is NULL.");
            yield break;
        }

        if (GameManager.instance.dialogueScreen != null)
        {
            Debug.Log("DialogueScreen found. Pausing game");
            GameManager.instance.statePause();
            GameManager.instance.DialogueScreen();

            UpdateDialogueScreen(
                "Enemy Defeated!",
                "Look, he dropped something. It might be useful. GET IT!",
                new Color(0.2f, 0.7f, 0.2f, 1f) // Green background
            );

            yield return new WaitForSecondsRealtime(0.5f); // Allow screen to fully render

            if (GameManager.instance.dialogueScreen.activeSelf)
            {
                Debug.Log("Dialogue screen is now active and displayed.");
            }
            else
            {
                Debug.LogError("Dialogue screen failed to activate.");
            }

            DropItem();
        }
        else
        {
            Debug.LogError("DialogueScreen is NULL. Check GameManager setup.");
        }

        Debug.Log("Destroying HealStationEnemy game object");
        Destroy(gameObject);
    }

    private void UpdateDialogueScreen(string header, string content, Color backgroundColor)
    {
        Debug.Log("Updating dialogue screen content");

        Transform dialogueScreen = GameManager.instance.dialogueScreen.transform;

        TMP_Text menuText = dialogueScreen.Find("Menu Text")?.GetComponent<TMP_Text>();
        TMP_Text dialogueText = dialogueScreen.Find("Text Area/Dialogue Text")?.GetComponent<TMP_Text>();
        UnityEngine.UI.Image background = dialogueScreen.GetComponent<UnityEngine.UI.Image>();

        if (menuText != null)
        {
            menuText.text = header;
            Debug.Log("Menu Text updated successfully.");
        }
        else
        {
            Debug.LogError("Menu Text not found in DialogueScreen!");
        }

        if (dialogueText != null)
        {
            dialogueText.text = content;
            Debug.Log("Dialogue Text updated successfully.");
        }
        else
        {
            Debug.LogError("Dialogue Text not found in DialogueScreen!");
        }

        if (background != null)
        {
            background.color = backgroundColor;
            Debug.Log("Background color updated successfully.");
        }
        else
        {
            Debug.LogError("Background Image not found in DialogueScreen!");
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