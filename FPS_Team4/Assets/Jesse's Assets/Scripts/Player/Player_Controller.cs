using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Controller : MonoBehaviour, IDamageable, IRecharge
{

    [Header("---- Camera Components ----")]
    [SerializeField] Camera_Controller camCtrl;
    [SerializeField] Transform playerCamera;
    private gameManager gm;

    [Header("---- Player Components ----")]
    [SerializeField] CharacterController playerCtrl;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask ignoreMask;

    [Header("---- Player Stats ____")]
    [SerializeField] public int HP;
    Vector3 moveDir;
    Vector3 playerVel;
    int origHP;

    [Header("---- Player Movement ----")]
    [SerializeField] public int moveSpeed;
    [SerializeField] public int sprintModifier;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpCount;
    [SerializeField] public float jumpSpeed;
    [SerializeField] public int gravity;
    private int origMoveSpeed;
    bool isSprinting = false;

    [Header("---- Grapple Hook ----")]
    [SerializeField] Vector3 grapplePoint;
    private GameObject heavyObject;
    public LineRenderer lineRenderer;
    [SerializeField] int maxDistance;
    [SerializeField] int grappleSpeed;
    [SerializeField] int pullSpeed;
    private int origGravity;
    private int origHookSpeed;

    [Header("---- Weapons ----")]
    [SerializeField] GameObject gunWeapon;
    [SerializeField] GameObject bladeWeapon;
    [SerializeField] float fireRate;
    
    private GunWeapon gunWeaponScript;


    [Header("---- Bools ----")]
    public bool grappleHookActive;
    private bool isGrappling;
    private bool pullingObject;
    private bool isFiring;

    public bool gunActive;
    public bool bladeActive;

    public bool canUseGun;
    public bool canUseBlade;

    private bool menuOpen = false;
    


    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = GetComponent<CharacterController>();

        gm = gameManager.instance;

        gunWeaponScript = gunWeapon.GetComponent<GunWeapon>();

        //For the grapple hook line
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; //Initial line

        //Getting originals
        origHP = HP;
        origMoveSpeed = moveSpeed;
        origGravity = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        Attack();
    }

    public void updatePlayerUI()
    {
        gm.playerHPBar.fillAmount = (float)HP / origHP;
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                gm.ShowWeaponMenu();
                gm.statePause();
            }
            else
            {
                gm.HideWeaponMenu();
                gm.stateUnpause();
            }
        }
        if (menuOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //ActivateGun();
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

                menuOpen = false;
                gm.HideWeaponMenu();
                gm.stateUnpause();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //ActivateBlade();
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

                menuOpen = false;
                gm.HideWeaponMenu();
                gm.stateUnpause();
            }
        }

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

        if(bladeActive)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                int randomAnim = Random.Range(1, 4);
                anim.SetBool("Swing1", randomAnim == 1);
                anim.SetBool("Swing2", randomAnim == 2);
                anim.SetBool("Swing3", randomAnim == 3);

                if(randomAnim == 1 || randomAnim == 2 || randomAnim == 3)
                {
                    StartCoroutine(BladeAnimationState(randomAnim));
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                anim.SetBool("Stab", true);
                StartCoroutine(StabeAnimationState());
            }
        }

        anim.SetBool("BladeActive", bladeActive);
        anim.SetBool("GunActive", gunActive);
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

    public void TakeDamage(int damage)
    {
        HP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if(HP < 0)
        {
            //You're Dead
        }
    }

    IEnumerator flashDamageScreen()
    {
        gm.playerDamageQue.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        
        gm.playerDamageQue.SetActive(false);
    }

    public void restoreHP(int amount)
    {
        HP = origHP;
        updatePlayerUI();
    }
}
