using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Controller : MonoBehaviour, IDamageable, IRecharge
{
    [Header("---- Bools ----")]
    public bool isSprinting = false;
    public bool isFiring;
    public bool gunActive;
    public bool bladeActive;
    public bool canUseGun;
    public bool canUseBlade;

    [Header("---- Player Stats ____")]
    [SerializeField] public float maxHP;
    [SerializeField] private float currentHP;
    [SerializeField] public float maxShield;
    [SerializeField] private float currentShield;

    [Header("---- Player Movement ----")]
    [SerializeField] public int moveSpeed;
    [SerializeField] public int sprintModifier;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpCount;
    [SerializeField] public float jumpSpeed;
    [SerializeField] public float gravity;
    Vector3 moveDir;
    Vector3 playerVel;
    private int origMoveSpeed;

    [Header("---- Camera Components ----")]
    [SerializeField] Camera_Controller camCtrl;
    [SerializeField] Transform playerCamera;
    private gameManager gm;
    public GameObject upgrader;
    public Upgrade_Menu upgradeScript;

    [Header("---- Player Components ----")]
    [SerializeField] CharacterController playerCtrl;
    [SerializeField] Upgrade_Menu upgradeMenu;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GrappleHookController grappleHook;

    [Header("---- Weapons ----")]
    [SerializeField] GameObject weaponMenu; //Menu for Weapons
    [SerializeField] GameObject skillMenu; //Menu for the Tools
    [SerializeField] GameObject gunWeapon;
    [SerializeField] GameObject bladeWeapon;
    [SerializeField] float fireRate;
    
    private GunWeapon gunWeaponScript;
    


    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = GetComponent<CharacterController>();

        gm = gameManager.instance;

        upgradeScript = upgrader.GetComponent<Upgrade_Menu>();
        gunWeaponScript = gunWeapon.GetComponent<GunWeapon>();

        maxHP = upgradeScript.GetUpgradedHealth(); //Players HP from Upgrades
        maxShield = upgradeScript.GetUpgradedShield(); //Players Shield Amount from Upgrades


        //Getting originals
        currentHP = maxHP;
        origMoveSpeed = moveSpeed;

        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {

        if (!gm.isPaused && !grappleHook.isGrappling)
        {
            PlayerMovement();
            Attack();
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
            gm.stateUnpause();
        }
        else
        {
            weaponMenu.SetActive(true);
            skillMenu.SetActive(false);
            gm.statePause();
        }
    }
    public void ToggleSkillMenu()
    {
        if (skillMenu.activeSelf)
        {
            skillMenu.SetActive(false);
            gm.stateUnpause();
        }
        else
        {
            skillMenu.SetActive(true);
            weaponMenu.SetActive(false);
            gm.statePause();
        }
    }
    void PlayerMovement()
    {

        //Player Grounded
        if (playerCtrl.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        //Walking and Sprinting
        moveDir = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        playerCtrl.Move(moveDir * moveSpeed * Time.deltaTime);
        Sprint();

        //Gravity
        playerCtrl.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        //Jumping Controls
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

            moveSpeed = isSprinting ? origMoveSpeed * sprintModifier : origMoveSpeed;
        }
    }

    void Attack()
    {
        fireRate = gunWeaponScript.shootRate;
        bool stabReady = anim.GetBool("StabReady");

        if(gunActive)
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

        if(bladeActive)
        {
            if(!stabReady)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    int randomAnim = Random.Range(1, 4);
                    anim.SetBool("Swing1", randomAnim == 1);
                    anim.SetBool("Swing2", randomAnim == 2);
                    anim.SetBool("Swing3", randomAnim == 3);

                    if (randomAnim == 1 || randomAnim == 2 || randomAnim == 3)
                    {
                        StartCoroutine(BladeAnimationState(randomAnim));
                    }
                }
            }

            if (Input.GetButton("Fire2"))
            {
                anim.SetBool("StabReady", true);

                if (Input.GetButtonDown("Fire1"))
                {
                    anim.SetBool("Stab", true);
                    StartCoroutine(StabeAnimationState());
                }
            }
            if (Input.GetButtonUp("Fire2"))
            {
                anim.SetBool("StabReady", false);
            }
        }

        anim.SetBool("BladeActive", bladeActive);
        anim.SetBool("GunActive", gunActive);
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

        //menuOpen = false;
        //gm.stateUnpause();
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

        //menuOpen = false;
        //gm.stateUnpause();
    }
    IEnumerator BladeAnimationState(int index)
    {
        bladeWeapon.GetComponent<BladeWeapon>().EnableCollider();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        bladeWeapon.GetComponent<BladeWeapon>().DisableCollider();

        anim.SetBool("Swing1", index == 1 && false);
        anim.SetBool("Swing2", index == 2 && false);
        anim.SetBool("Swing3", index == 3 && false);
    }
    IEnumerator StabeAnimationState()
    {
        bladeWeapon.GetComponent<BladeWeapon>().EnableCollider();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        bladeWeapon.GetComponent<BladeWeapon>().DisableCollider();

        anim.SetBool("Stab", false);
    }
    IEnumerator FireCouroutine()
    {
        isFiring = true;

        while(isFiring)
        {
            gunWeaponScript.Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }
    public void TakeDamage(float damage)
    {
        if(maxShield > 0)
        {
            currentShield -= damage;
            updatePlayerUI();
            StartCoroutine(flashSheildHItScreen());
        }
        
        currentHP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        //if(currentHP <= 0)
        //{
        //    //You're Dead
        //}
    }
    IEnumerator flashDamageScreen()
    {
        gm.playerDamageQue.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        
        gm.playerDamageQue.SetActive(false);
    }
    IEnumerator flashSheildHItScreen()
    {
        gm.playerShieldHitQue.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        gm.playerShieldHitQue.SetActive(false);
    }
    public void restoreHP(int amount)
    {
        currentHP = maxHP;
        updatePlayerUI();
    }
}
