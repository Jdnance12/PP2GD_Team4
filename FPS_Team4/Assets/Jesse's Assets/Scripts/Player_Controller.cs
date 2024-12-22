using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    [Header("---- Camera Components ----")]
    [SerializeField] Camera_Controller camCtrl;
    [SerializeField] Transform playerCamera;

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

    public bool grappleHookActive;
    private bool isGrappling;
    private bool pullingObject;

    public bool gunActive;
    public bool bladeActive;
    


    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = GetComponent<CharacterController>();

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

        if(Input.GetKeyDown(KeyCode.Q))
        {
            bladeActive = !bladeActive;
            if(bladeActive)
            {
                gunActive = false;
            }
        }
       
        if(Input.GetKeyDown(KeyCode.E))
        {
            gunActive = !gunActive;
            if(gunActive)
            {
                bladeActive = false;
            }
        }

        anim.SetBool("BladeActive", bladeActive);
        anim.SetBool("GunActive", gunActive);
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
}
