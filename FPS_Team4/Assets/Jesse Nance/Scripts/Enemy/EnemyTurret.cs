using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour, IDamageable, IDisrupt
{

    public bool playerInRange;
    public bool isShooting;
    public bool isDisrupted;

    [SerializeField] GameManager gm;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject partsPrefab;
    [SerializeField] Renderer bodyRenderer;
    [SerializeField] Renderer gunRenderer;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletPos;

    [SerializeField] float HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float angleToPlayer;
    [SerializeField] float shootRate;
    [SerializeField] float disruptDuration;

    private Color origBodyColor;
    private Color origGunColor;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;

        origBodyColor = bodyRenderer.material.color;
        origGunColor = gunRenderer.material.color;
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
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDisrupted)
        {
            if (playerInRange)
            {
                Vector3 playerDir = player.transform.position + Vector3.up * 2.0f - transform.position;
                angleToPlayer = Vector3.Angle(playerDir, transform.forward);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, playerDir, out hit))
                {
                    if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
                    {
                        FaceTarget();

                        if (!isShooting)
                        {
                            StartCoroutine(Shoot());
                        }
                    }
                }
            }
        }      
    }

    void FaceTarget()
    {
        //Rotate the Body
        Vector3 playerDir = player.transform.position + Vector3.up * 2.0f - transform.position;

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);


        Vector3 aimDir = player.transform.position + Vector3.up * 2.0f - gunObject.transform.position;

        Quaternion gunRot = Quaternion.LookRotation(aimDir);
        gunObject.transform.rotation = Quaternion.Lerp(gunObject.transform.rotation, gunRot, Time.deltaTime * faceTargetSpeed);

        Vector3 limitRotation = gunObject.transform.localEulerAngles;
        limitRotation.x = Mathf.Clamp(limitRotation.x, -30f, 30f);
    }
    IEnumerator Shoot()
    {
        isShooting = true;

        Instantiate(bulletPrefab, bulletPos.position, gunObject.transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }


    public void causeDisrupt()
    {
        StartCoroutine(Disrupted());
    }
    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(flashRed()); // Call flash red when damage is taken

        if (HP <= 0)
        {
            Instantiate(partsPrefab, transform.position, Quaternion.identity); // Drops the parts currency when the enemy is destoryed
            Destroy(gameObject);
        }
    }
    IEnumerator Disrupted()
    {
        isDisrupted = true;
        bodyRenderer.material.color = Color.blue;
        gunRenderer.material.color = Color.blue;

        isShooting = false;

        yield return new WaitForSeconds(disruptDuration);


        bodyRenderer.material.color = origBodyColor;
        gunRenderer.material.color = origGunColor;
        isDisrupted = false;
    }
    IEnumerator flashRed()
    {
        bodyRenderer.material.color = Color.red;
        gunRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f); //Turns red for 1 second

        bodyRenderer.material.color = origBodyColor;
        gunRenderer.material.color = origGunColor;
    }
}
