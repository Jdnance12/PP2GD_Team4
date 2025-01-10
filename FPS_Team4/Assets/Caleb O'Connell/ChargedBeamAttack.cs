using UnityEngine;

public class ChargedBeamAttack : MonoBehaviour
{
    [Header("Beam Settings")]
    public GameObject beamPrefab;          // Prefab for the beam
    public Transform firePoint;            // Point where the beam spawns
    public float maxChargeTime = 3f;       // Maximum time to fully charge the beam
    public float minBeamDamage = 10f;      // Minimum damage of the beam
    public float maxBeamDamage = 50f;      // Maximum damage of the beam

    [Header("UI Settings")]
    public UnityEngine.UI.Image chargeBar; // UI image for charge level

    private float chargeTime;              // Current charge time
    private bool isCharging;               // Is the beam being charged?

    void Update()
    {
        HandleInput();
        UpdateChargeBar();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCharging();
        }

        if (Input.GetButton("Fire1"))
        {
            ChargeBeam();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            FireBeam();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTime = 0f;
    }

    private void ChargeBeam()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
        }
    }

    private void FireBeam()
    {
        if (isCharging)
        {
            isCharging = false;

            // Calculate beam damage based on charge time
            float chargePercent = chargeTime / maxChargeTime;
            float beamDamage = Mathf.Lerp(minBeamDamage, maxBeamDamage, chargePercent);

            // Spawn and initialize the beam
            GameObject beam = Instantiate(beamPrefab, firePoint.position, firePoint.rotation);
            BeamController beamController = beam.GetComponent<BeamController>();
            if (beamController != null)
            {
                beamController.Initialize(beamDamage);
            }

            // Reset charge time
            chargeTime = 0f;
        }
    }

    private void UpdateChargeBar()
    {
        if (chargeBar != null)
        {
            chargeBar.fillAmount = chargeTime / maxChargeTime;
        }
    }
}
