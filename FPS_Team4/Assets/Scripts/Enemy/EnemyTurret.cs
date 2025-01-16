using System.Collections;
using UnityEngine;

public class TurretEnemy : MonoBehaviour, IDamageable, IDisrupt
{
    [Header("---- Detection Stats ----")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] LayerMask detectionLayer;

    [Header("---- Attack Stats ----")]
    [SerializeField] float fireRate = 2f;
    [SerializeField] float empDuration = 5f;
    [SerializeField] float bulletDamage = 10f;

    [Header("---- Components ----")]
    [SerializeField] Transform turretHead;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject empPrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Renderer turretRenderer;

    [Header("---- Health ----")]
    [SerializeField] float HP = 100f;

    private GameObject player;
    private bool isPlayerDetected = false;
    private bool isShooting = false;
    private bool isDisrupted = false;

    private Color origColor;

    void Start()
    {
        player = GameManager.instance.player;
        origColor = turretRenderer.material.color;
    }

    void Update()
    {
        if (isDisrupted) return;

        DetectPlayer();

        if (isPlayerDetected)
        {
            RotateTowardsPlayer();

            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {
            Patrol();
        }
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                isPlayerDetected = true;
                return;
            }
        }

        isPlayerDetected = false;
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.transform.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        turretHead.rotation = Quaternion.RotateTowards(turretHead.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        turretHead.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        if (empPrefab != null)
        {
            GameObject emp = Instantiate(empPrefab, firePoint.position, turretHead.rotation);
            EMP empScript = emp.GetComponent<EMP>();
            if (empScript != null)
            {
                empScript.SetDuration(empDuration);
            }
        }
        else if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, turretHead.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
            }
        }

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void causeDisrupt()
    {
        StartCoroutine(Disrupted());
    }

    IEnumerator Disrupted()
    {
        isDisrupted = true;
        turretRenderer.material.color = Color.blue;

        yield return new WaitForSeconds(empDuration);

        turretRenderer.material.color = origColor;
        isDisrupted = false;
    }

    IEnumerator FlashRed()
    {
        turretRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        turretRenderer.material.color = origColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}