using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, iDamage, IRecharge
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] cameraController camController;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    public int maxHP = 20;
    [SerializeField][Range(1, 10)] public int HP;
    [SerializeField][Range(1, 10)] float speed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField] float crouchHeight;
    [SerializeField][Range(2, 5)] float crouchMod;
    [SerializeField][Range(1, 3)] float jumpMax;
    [SerializeField][Range(5, 20)] float jumpSpeed;
    [SerializeField][Range(15, 40)] float gravity;
    [SerializeField][Range(1, 5)] int fallDmgHeight;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Grapple Hook")]
    public GameObject heavyObject;
    public LineRenderer lineRenderer;
    public Vector3 grapplePoint;

    public float maxDistance;
    public float hookSpeed;
    private float originalGravity;
    private float originalHookSpeed;

    private bool isGrappling = false;
    private bool pullingObject = false;
    private bool drawLine = false;
    public bool grappleHookActive;

    [Header("Input Actions")]
    public InputActionReference weaponMenuAction;

    [Header("Weapon Stats")]
    [SerializeField] GameObject weaponModel;
    [SerializeField] List<Weapon> weaponList = new List<Weapon>();
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] float stunTimer;
    int weaponListPos;

    [Header("Temp Variables")]
    [SerializeField] public bool canDoubleJump;

    [Header("Abilities")]
    public bool canUnjamDoors = false; // Tracks if the player can unjam doors
    
    //Local variables

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float lastGroundedHeight;
    float targetFOV = 60f;
    private bool hasLanded = false; // Tracks if the player has landed for first sceen

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isJumping;
    bool isStunned;
    private bool firstDrop = true;
    [SerializeField] bool wasGrounded;

    public bool hasWeapon = false; // track if the player has a weapon

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        originalGravity = gravity;

        HPOrig = HP;
        lastGroundedHeight = transform.position.y; // Initialize to starting height
        //GameManager.instance.UpdatePlayerHealth(HP, maxHP); //REQUIRED TO UPDATE THE UI THROUGH THE GAME MANAGER FOR DYNAMIC UI

        Debug.Log("Start method called in playerController.");

        if (hasWeapon && weaponMenuAction != null && weaponMenuAction.action != null)
        {
            Debug.Log("weaponMenuAction is initialized in Start.");
            weaponMenuAction.action.Enable();
        }
        else
        {
            Debug.Log("WeaponMenuAction or weaponMenuAction.action is null in Start or player has no weapon.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if(!GameManager.instance.isPaused)
        { 
            if(!isGrappling)
            {
                movement();
            }
            //movement();
            selectWeapon();

            if(Input.GetButton("Grapple") && grappleHookActive)
            {
                LaunchHook();
                camController.enabled = false;

                if(isGrappling)
                {
                    Grapple();
                    gravity = 0f;
                }
                else if(pullingObject && heavyObject != null)
                {
                    PullObject();
                }             
            }

            if(Input.GetButtonUp("Grapple"))
            {
                isGrappling = false;
                pullingObject = false;
                drawLine = false;
                lineRenderer.positionCount = 0;
                gravity = originalGravity;
                camController.enabled = true;
            }

            UpdateLine();
        }


        //Updates FOV with lerp
        //Camera playerCam = playerCamera.GetComponent<Camera>();
        //playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            isJumping = false;
            jumpCount = 0;
            playerVel = Vector3.zero;
            //checkFallDamage();
        }

        jump();
        sprint();
        crouch();

        moveDir = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * speed * Time.deltaTime);

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Controls

        if(weaponList.Count > 0 )
        {
            if (Input.GetButton("Fire1") && !isShooting)
            {
                StartCoroutine(shoot());
            }
            if (Input.GetButtonDown("Fire2"))
            {
                zoom(30f);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                zoom(60f);
            }
        }
    }

    void zoom(float target)
    {
        targetFOV = target;
    }

    void jump()
    {
        if (canDoubleJump)
            jumpMax = 2;
        else
            jumpMax = 1;

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && !isCrouching)
        {
            isJumping = true;
            wasGrounded = false;
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            if (isCrouching)
            {
                // Stop crouching if sprint is pressed
                isCrouching = false;
                speed *= crouchMod;
                crouchHeight = 1f; // Reset camera height to standing
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    crouchHeight,
                    playerCamera.localPosition.z
                );
            }

            isSprinting = !isSprinting;
            speed = (isSprinting ? speed *= sprintMod : speed /= sprintMod);
        }
    }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch") && !isJumping)
        {
            if (isSprinting)
            {
                // Stop sprinting if crouch is pressed
                isSprinting = false;
                speed /= sprintMod; // Reset speed to walk speed
            }
            isCrouching = !isCrouching;
            crouchHeight = isCrouching ? 0 : 1;
            speed = (isCrouching ? speed /= crouchMod : speed *= crouchMod);
        }

        playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                crouchHeight,
                playerCamera.localPosition.z
            );
    }

    IEnumerator shoot()
    {
        isShooting = true;

        //shoot code
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);
            iDamage dmg = hit.collider.GetComponent<iDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    public void takeDamage(int amount) // THIS IS SO THE PLAYER REPORTS DAMAGE TO THE GAME MANAGER AND UPDATES THE UI
    {
        Debug.Log($"Player takes {amount} damage. Current HP: {HP}, Max HP: {maxHP}");

        HP -= amount;

        if (HP < 0) // Safety net to stop HP at zero
        {
            HP = 0;
        }

        if (GameManager.instance != null) // Notify the GameManager to update the health bar UI
        {
            GameManager.instance.UpdatePlayerHealth(HP, maxHP);
        }

        StartCoroutine(flashScreenDamage()); // Flash the red damage screen

        if (HP <= 0) // Check for death
        {
            GameManager.instance.youLose();
        }
    }
    public void takeEMP(int amount)
    {
        StartCoroutine(Stun(stunTimer));
    }

    IEnumerator Stun(float stunDuration)
    {
        isStunned = true;
        isShooting = false;

        GameManager.instance.OnStunBegin(); // Notify GameManager of stun

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;

        GameManager.instance.OnStunEnd(); // Notify GameManager of stun end
    }

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void getWeaponStats(Weapon weapon)
    {
        weaponList.Add(weapon);

        shootDamage = weapon.shootDamage;
        shootDist = weapon.shootDist;
        shootRate = weapon.shootRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponListPos < weaponList.Count - 1)
        {
            weaponListPos++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponListPos > 0)
        {
            weaponListPos--;
            changeWeapon();
        }
    }

    public void changeWeapon()
    {
        shootDamage = weaponList[weaponListPos].shootDamage;
        shootDist = weaponList[weaponListPos].shootDist;
        shootRate = weaponList[weaponListPos].shootRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[weaponListPos].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[weaponListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    //public void updatePlayerUI()
    // {
    //     GameManager.instance.playerHPBar.fillAmount = (float) HP / HPOrig;
    // }

public void toggleDoubleJump()
    {
        canDoubleJump = true; // Gives player double jump
    }

    public void restoreHP(int amount)
    {
        if ((HP + amount) <= maxHP)
        {
            HP += amount;
            GameManager.instance.UpdatePlayerHealth(HP, maxHP); //REQUIRED TO UPDATE THE UI THROUGH THE GAME MANAGER FOR DYNAMIC UI
        }
        else if ((HP + amount) > maxHP)
        {
            HP = maxHP;
            GameManager.instance.UpdatePlayerHealth(HP, maxHP); //REQUIRED TO UPDATE THE UI THROUGH THE GAME MANAGER FOR DYNAMIC UI
        }
    }

    public void ResetPlayerState()
    {
        Debug.Log("Resetting player shooting state after unpause.");
        isShooting = false; // Reset shooting flag
    }

    public void HealToMax()
    {
        HP = maxHP; // Set health to maximum
        Debug.Log($"Player healed to full health: {HP}/{maxHP}");

        // Update health bar UI through the GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePlayerHealth(HP, maxHP);
        }
        else
        {
            Debug.LogError("GameManager instance is NULL. Cannot update HP UI.");
        }
    }

    // Method to handle door unjamming by the player
    public void UnjamDoor()
    {
        if (canUnjamDoors)
        {
            Debug.Log("Player is unjamming the door");
            
        }
        else
        {
            Debug.Log("Player cannot unjam doors yet. Find the toolbox!");
        }
    }

    void LaunchHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance))
        {
            if(hit.collider.CompareTag("Heavy Object"))
            {
                pullingObject = true;
                heavyObject = hit.collider.gameObject;
                lineRenderer.positionCount = 2;
            }
            else
            {
                grapplePoint = hit.point;
                isGrappling = true;
                lineRenderer.positionCount = 2;

            }
        }
    }

    void Grapple()
    {
        transform.position = Vector3.MoveTowards(transform.position,grapplePoint, hookSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position,grapplePoint) < 1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + 1);
            isGrappling = false;
            lineRenderer.positionCount = 0;
        }
    }

    void PullObject()
    {
        heavyObject.transform.position = Vector3.MoveTowards(heavyObject.transform.position, transform.position, hookSpeed * Time.deltaTime);
        if (Vector3.Distance(heavyObject.transform.position, transform.position) < 1f)
        {
            pullingObject = false;
            lineRenderer.positionCount = 0;
        }
    }

    void UpdateLine()
    {
        if(lineRenderer.positionCount > 0)
        {
            lineRenderer.SetPosition(0, transform.position);
            if(pullingObject && heavyObject != null)
            {
                lineRenderer.SetPosition(1, heavyObject.transform.position);
            }
            else
            {
                lineRenderer.SetPosition(1, grapplePoint);
            }
        }
    }
}