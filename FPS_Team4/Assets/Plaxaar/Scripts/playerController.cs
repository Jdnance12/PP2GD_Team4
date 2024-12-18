using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, IDamage, IRecharge
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    public int maxHP = 20;
    [SerializeField][Range(1, 10)] public int HP;
    [SerializeField][Range(1, 5)] float speed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField] float crouchHeight;
    [SerializeField][Range(2, 5)] float crouchMod;
    [SerializeField][Range(1, 3)] float jumpMax;
    [SerializeField][Range(5, 20)] float jumpSpeed;
    [SerializeField][Range(15, 40)] float gravity;
    [SerializeField][Range(1, 5)] int fallDmgHeight;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Input Actions")]
    public InputActionReference weaponMenuAction;

    [Header("Gun Stats")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("Temp Variables")]
    [SerializeField] public bool canDoubleJump;
    
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
    private bool firstDrop = true;
    [SerializeField] bool wasGrounded;

    public bool hasWeapon = false; // track if the player has a weapon

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

        movement();
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

    //void OnEnable()
    //{
    //    Debug.Log("OnEnable called in playerController.");

    //    if (hasWeapon && weaponMenuAction != null && weaponMenuAction.action != null)
    //    {
    //        weaponMenuAction.action.performed += OnWeaponMenuPerformed; // When Q is held for 0.25s
    //        weaponMenuAction.action.canceled += OnWeaponMenuCanceled;   // When Q is released
    //        weaponMenuAction.action.Enable();
    //        Debug.Log("weaponMenuAction successfully initialized in OnEnable.");
    //    }
    //    else
    //    {
    //        Debug.LogError("weaponMenuAction or weaponMenuAction.action is null in OnEnable.");
    //    }
    //}

    //void OnDisable()
    //{
    //    Debug.Log("OnDisable called in playerController.");

    //    if (hasWeapon && weaponMenuAction != null && weaponMenuAction.action != null)
    //    {
    //        weaponMenuAction.action.performed -= OnWeaponMenuPerformed;
    //        weaponMenuAction.action.canceled -= OnWeaponMenuCanceled;
    //        weaponMenuAction.action.Disable();
    //        Debug.Log("weaponMenuAction successfully disabled.");
    //    }
    //    else
    //    {
    //        Debug.LogError("weaponMenuAction or weaponMenuAction.action is null in OnDisable.");
    //    }
    //}

    // Called when button hold is performed
    private void OnWeaponMenuPerformed(InputAction.CallbackContext context)
    {
        GameManager.instance.WeaponMenuActive(context);
    }

    // Called when button is released
    private void OnWeaponMenuCanceled(InputAction.CallbackContext context)
    {
        GameManager.instance.WeaponMenuNotActive(context);
    }

    void zoom(float target)
    {
        Camera playerCam = playerCamera.GetComponent<Camera>();
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, target, zoomSpeed * Time.deltaTime);
    }

    //void checkFallDamage()
    //{
    //    if (!wasGrounded && controller.isGrounded && !firstDrop)
    //    {
    //        float fallDistance = lastGroundedHeight - transform.position.y;
            
    //        if (Mathf.Abs(fallDistance) < Mathf.Epsilon)
    //        {
    //            fallDistance = 0.0f; // Treat near-zero as zero
    //        }


    //        //Check if fall distance is higher than min fall height
    //        if (fallDistance > fallDmgHeight)
    //        {
    //            int damage;
    //            //Scales Damage linearly
    //            if (fallDistance > 15)
    //            {
    //                damage = int.MaxValue;
    //            }
    //            else
    //            {
    //                damage = Mathf.CeilToInt(fallDistance / fallDmgHeight);
    //            }

    //            takeDamage(damage);
    //        }
    //    }
    //    else
    //    {
    //        firstDrop = false;
    //    }

    //    wasGrounded = controller.isGrounded;

    //    if (controller.isGrounded)
    //    {
    //        lastGroundedHeight = transform.position.y;
    //    }

    //}

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
            IDamage dmg = hit.collider.GetComponent<IDamage>();
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

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
     {
         GameManager.instance.playerHPBar.fillAmount = (float) HP / HPOrig;
     }

public void toggleDoubleJump()
    {
        canDoubleJump = true; // Gives player double jump
    }

    //changed code
    //public void restoreHP(int amount)
    //{
    //    if ((HP + amount) <= maxHP)
    //    {
    //        HP += amount;
    //        GameManager.instance.UpdatePlayerHealth(HP, maxHP); //REQUIRED TO UPDATE THE UI THROUGH THE GAME MANAGER FOR DYNAMIC UI
    //    }
    //    else if ((HP + amount) > maxHP)
    //    {
    //        HP = maxHP;
    //        GameManager.instance.UpdatePlayerHealth(HP, maxHP); //REQUIRED TO UPDATE THE UI THROUGH THE GAME MANAGER FOR DYNAMIC UI
    //    }
    //}

    public void restoreHP(int amount)
    {
        if ((HP + amount) <= maxHP)
        {
            HP += amount;
            updatePlayerUI();
        }
        else if ((HP + amount) > maxHP)
        {
            HP = maxHP;
            updatePlayerUI();
        }
    }



    public void ResetPlayerState()
    {
        Debug.Log("Resetting player shooting state after unpause.");
        isShooting = false; // Reset shooting flag
    }

    //public void HealToMax()
    //{
    //    HP = maxHP; // Set health to maximum
    //    Debug.Log($"Player healed to full health: {HP}/{maxHP}");

    //    // Update health bar UI through the GameManager
    //    if (GameManager.instance != null)
    //    {
    //        GameManager.instance.UpdatePlayerHealth(HP, maxHP);
    //    }
    //    else
    //    {
    //        Debug.LogError("GameManager instance is NULL. Cannot update HP UI.");
    //    }
    //}
}