using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable, IRecharge
{
    [Header("----- Bools -----")]
    public bool isSprinting = false;
    public bool isFiring;
    public bool isCrouching;
    public bool gunActive;
    public bool bladeActive;

    [Header("----- Health Stats -----")]
    // Health Stats
    [SerializeField] public float maxHP = 100;
    [SerializeField] private float currentHP;
    [SerializeField] public float maxShield = 0;
    [SerializeField] private float currentShield;

    [Header("---- Movement Stats ----")]
    [SerializeField] float speed = 7;
    [SerializeField] float sprintMod = 2;
    [SerializeField] public int jumpMax = 2;
    [SerializeField] int jumpCount = 0;
    [SerializeField] int jumpSpeed = 20;
    [SerializeField] public float gravity = 40;
    Vector3 moveDir;
    Vector3 playerVel;
    private float origMoveSpeed;

    [Header("---- Crouch Stats ----")]
    //[SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float standSpeed = 7.0f;

      [Header("---- Slope Handling ----")]
    [SerializeField] private float slopeForce = 5f;
    [SerializeField] private float slopeForceRayLength = 1.5f;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float fireRate;

    [Header("----- Player Components -----")]
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GameObject gameManager;
    [SerializeField] CharacterController playerCtrl;
    [SerializeField] GameObject playerCamera;
    [SerializeField] Camera_Controller camCtrl;
    [SerializeField] UpgradeManager upgradeManager;
    [SerializeField] Animator armsAnim;
    [SerializeField] Animator bodyAnim;
    private Transform playerCamTrans;
    private GameManager gm;

    [Header("---- Weapon Components ----")]
    [SerializeField] GameObject gunWeapon;
    [SerializeField] GameObject bladeWeapon;
    [SerializeField] GameObject grappleHook;
    [SerializeField] GrappleHookController grappleHookCtrl;
    [SerializeField] GameObject weaponMenu;
    [SerializeField] GameObject skillMenu;
    private GunWeapon gunScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager"); // Referenciing the object that has the Game Manager and other scripts on it.
        gm = GameManager.instance; // Reference for the Game Manager

        playerCtrl = GetComponent<CharacterController>(); // Getting the controller so the player can move

        playerCamera = Camera.main.gameObject; // Referencing the players camera to get it's script and transform.
        camCtrl = playerCamera.GetComponent<Camera_Controller>();
        playerCamTrans = playerCamera.transform;

        upgradeManager = gameManager.GetComponent<UpgradeManager>(); // Referencing the script for the players upgrades

        gunWeapon = GameObject.Find("Gun Weapon Object"); gunScript = gunWeapon.GetComponent<GunWeapon>();
        bladeWeapon = GameObject.Find("Blade Weapon Object");
        weaponMenu = GameObject.Find("Weapon Menu"); weaponMenu.SetActive(false);
        skillMenu = GameObject.Find("Skill Menu"); skillMenu.SetActive(false);

        grappleHook = GameObject.Find("Grapple Hook Object");
        grappleHookCtrl = grappleHook.GetComponent<GrappleHookController>();

        //Base Stats
        maxHP = 100;
        currentHP = maxHP;
        maxShield = 0;
        currentShield = maxShield;

        origMoveSpeed = speed;

        standHeight = playerCtrl.height;

        updatePlayerUI();

    }

    // Update is called once per frame
    void Update()
    {

        if (!gm.isPaused && !grappleHookCtrl.isGrappling)
        {
            PlayerMovement();
            AdjustToSlope();
            Attack();
            HandleCrouch();
        }
        if (isCrouching == false)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleWeaponMenu();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSkillMenu();
        }
    }

    public void updatePlayerUI()
    {
        gm.playerHPBar.fillAmount = currentHP / maxHP;
        gm.playerShieldBar.fillAmount = currentShield / maxShield;

    }
    public void UpdateMaxHP(float newMaxHP)
    {
        maxHP = newMaxHP;
        currentHP = maxHP;

        updatePlayerUI();
    }
    public void UpdateMaxShield(float newMaxShield)
    {
        maxShield = newMaxShield;
        currentShield = maxShield;

        updatePlayerUI();
    }
    public void ToggleWeaponMenu()
    {
        if (weaponMenu.activeSelf)
        {
            weaponMenu.SetActive(false);
            gm.WeaponMenuUnPaused();
        }
        else
        {
            weaponMenu.SetActive(true);
            skillMenu.SetActive(false);
            gm.GamePaused();
        }
    }
    public void ToggleSkillMenu()
    {
        if (skillMenu.activeSelf)
        {
            skillMenu.SetActive(false);
            gm.WeaponMenuUnPaused();
        }
        else
        {
            skillMenu.SetActive(true);
            weaponMenu.SetActive(false);
            gm.GamePaused();
        }
    }
    public void ToggleGun()
    {
        gunActive = !gunActive;

        if (gunActive)
        {
            bladeActive = false;
            gunWeapon.SetActive(true);
            bladeWeapon.SetActive(false);

            gm.gunReticule.gameObject.SetActive(true);
            gm.bladeReticule.gameObject.SetActive(false);

        }
        else
        {
            gm.gunReticule.gameObject.SetActive(false);
        }

        ToggleWeaponMenu();
    }
    public void ToggleBlade()
    {
        bladeActive = !bladeActive;
        if (bladeActive)
        {

            gunActive = false;
            bladeWeapon.SetActive(true);
            gunWeapon.SetActive(false);

            gm.gunReticule.gameObject.SetActive(false);
            gm.bladeReticule.gameObject.SetActive(true);
        }
        else
        {
            gm.bladeReticule.gameObject.SetActive(false);
        }

        ToggleWeaponMenu();
    }
    void PlayerMovement()
    {
        //Gravity
        playerCtrl.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        //Player Movement
        moveDir = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        playerCtrl.Move(moveDir * speed * Time.deltaTime);

        Sprint();
    }

    void AdjustToSlope()
    {
        if (playerCtrl.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeForceRayLength))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > playerCtrl.slopeLimit)
                {
                    Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.down), hit.normal);
                    playerCtrl.Move(slopeDirection * slopeForce * Time.deltaTime);
                  }
               }
            }
        }

    public void Jump()
    {

        if (playerCtrl.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {

            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = !isSprinting;

            speed = isSprinting ? origMoveSpeed * sprintMod : origMoveSpeed;
        }
    }
    void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;

            if(isCrouching)
            {
                playerCtrl.height = playerCtrl.height / 2f;
                playerCtrl.center = new Vector3(playerCtrl.center.x, 1, playerCtrl.center.z);
                speed = crouchSpeed;
                bodyAnim.SetBool("IsCrouched", true);
            }
            else
            {
                playerCtrl.height = standHeight;
                playerCtrl.center = new Vector3(playerCtrl.center.x, 2, playerCtrl.center.z);
                speed = standSpeed;
                bodyAnim.SetBool("IsCrouched", false);
            }
        }
    }
    void Attack()
    {
        fireRate = gunScript.shootRate;
        bool stabReady = armsAnim.GetBool("StabReady");

        if (gunActive)
        {
            if (Input.GetButton("Fire1") && !isFiring)
            {
                StartCoroutine(FireCouroutine());
            }
            if (Input.GetButtonUp("Fire1"))
            {
                isFiring = false;
            }
        }
        if (bladeActive)
        {
            if (!stabReady)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    int randomAnim = Random.Range(1, 4);
                    armsAnim.SetBool("Swing1", randomAnim == 1);
                    armsAnim.SetBool("Swing2", randomAnim == 2);
                    armsAnim.SetBool("Swing3", randomAnim == 3);

                    if (randomAnim == 1 || randomAnim == 2 || randomAnim == 3)
                    {
                        StartCoroutine(BladeAnimationState(randomAnim));
                    }
                }
            }

            // Stabbing
            if (Input.GetButton("Fire2"))
            {
                armsAnim.SetBool("StabReady", true);

                if (Input.GetButtonDown("Fire1"))
                {
                    armsAnim.SetBool("Stab", true);
                    StartCoroutine(StabeAnimationState());
                }
            }
            if (Input.GetButtonUp("Fire2"))
            {
                armsAnim.SetBool("StabReady", false);
            }
        }

        armsAnim.SetBool("BladeActive", bladeActive);
        armsAnim.SetBool("GunActive", gunActive);
    }
    IEnumerator BladeAnimationState(int index)
    {
        bladeWeapon.GetComponent<BladeWeapon>().EnableCollider();
        yield return new WaitForSeconds(armsAnim.GetCurrentAnimatorStateInfo(0).length);
        bladeWeapon.GetComponent<BladeWeapon>().DisableCollider();

        armsAnim.SetBool("Swing1", index == 1 && false);
        armsAnim.SetBool("Swing2", index == 2 && false);
        armsAnim.SetBool("Swing3", index == 3 && false);
    }
    IEnumerator StabeAnimationState()
    {
        bladeWeapon.GetComponent<BladeWeapon>().EnableCollider();
        yield return new WaitForSeconds(armsAnim.GetCurrentAnimatorStateInfo(0).length);
        bladeWeapon.GetComponent<BladeWeapon>().DisableCollider();

        armsAnim.SetBool("Stab", false);
    }
    IEnumerator FireCouroutine()
    {
        isFiring = true;

        while (isFiring)
        {
            gunScript.Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }
    public void restoreHP(int amount)
    {
        //currentHP += amount;
        currentHP = maxHP;
        updatePlayerUI();
    }
    public void TakeDamage(float damage)
    {
        if (maxShield > 0)
        {
            currentShield -= damage;
            //updatePlayerUI();
            StartCoroutine(flashSheildHItScreen());
        }

        currentHP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());
    }
    IEnumerator flashDamageScreen()
    {
        gm.playerHealthHitImage.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        gm.playerHealthHitImage.SetActive(false);
    }
    IEnumerator flashSheildHItScreen()
    {
        gm.playerShieldHitImage.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        gm.playerShieldHitImage.SetActive(false);
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }
}
