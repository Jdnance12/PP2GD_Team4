using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Controller : MonoBehaviour
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
    [SerializeField] public int maxHP;
    Vector3 moveDir;
    Vector3 playerVel;

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
        origMoveSpeed = moveSpeed;
        origGravity = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        Attack();
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

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        fireRate = gunWeaponScript.shootRate;

        if (Input.GetKeyDown(KeyCode.E))
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                gm.ShowWeaponMenu();
            }
            else
            {
                gm.HideWeaponMenu();
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


        anim.SetBool("BladeActive", bladeActive);
        anim.SetBool("GunActive", gunActive);
    }

    void ActivateGun()
    {
        gunActive = true;
        gunWeapon.SetActive(true);
        bladeWeapon.SetActive(false);

        gm.gunReticule.gameObject.SetActive(true);
        gm.bladeReticule.gameObject.SetActive(false);

        menuOpen = false;
        gm.HideWeaponMenu();
    }
    void ActivateBlade()
    {
        bladeActive = true;
        gunActive = false;

        bladeWeapon.SetActive(true);
        gunWeapon.SetActive(false);

        gm.gunReticule.gameObject.SetActive(false);
        gm.bladeReticule.gameObject.SetActive(true);

        menuOpen = false;
        gm.HideWeaponMenu();
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
}
