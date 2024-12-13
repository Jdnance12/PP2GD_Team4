using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 5)] float speed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField] float crouchHeight;
    [SerializeField][Range(2, 5)] float crouchMod;
    [SerializeField][Range(1, 3)] float jumpMax;
    [SerializeField][Range(5, 20)] float jumpSpeed;
    [SerializeField][Range(15, 40)] float gravity;
    [SerializeField][Range(5, 10)] int fallDmgHeight;

    [Header("Gun Stats")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("Temp Variables")]
    [SerializeField] bool canDoubleJump;
    
    //Local variables

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float lastGroundedHeight;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isJumping;
    [SerializeField] bool wasGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
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
            checkFallDamage();
        }

        jump();
        sprint();
        crouch();

        moveDir = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * speed * Time.deltaTime);

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot());
        }
        if (Input.GetButtonDown("Fire2"))
        {
            playerCamera.GetComponent<Camera>().fieldOfView = 30f;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            playerCamera.GetComponent<Camera>().fieldOfView = 60f;
        }
    }

    void checkFallDamage()
    {
        if (!wasGrounded && controller.isGrounded)
        {
            float fallDistance = lastGroundedHeight - transform.position.y;

            //Check if fall distance is higher than min fall height
            if (fallDistance > fallDmgHeight)
            {
                int damage;
                //Scales Damage linearly
                if (fallDistance > 15)
                {
                    damage = int.MaxValue;
                }
                else
                {
                    damage = Mathf.CeilToInt(fallDistance / fallDmgHeight);
                }

                ////Scales Damage on 3 different scales. Still needs fine tuning and fixing.
                //switch (fallDistance / fallDmgHeight)
                //{
                //    case 1:
                //        takeDamage(fallDmgHeight);
                //        Debug.Log("Light fall damage taken");
                //        break;
                //    case 2:
                //        takeDamage(fallDmgHeight * 2);
                //        Debug.Log("Moderate fall damage taken");
                //        break;
                //    case 3:
                //        takeDamage(fallDmgHeight * 3);
                //        Debug.Log("Heavy fall damage taken");
                //        break;
                //    default:
                //        if (fallDistance / fallDmgHeight > 3)
                //        {
                //            Debug.Log("Full HP fall damage taken");
                //            takeDamage(HP);
                //        }
                //        break;
                //}

                takeDamage(damage);
            }
        }

        wasGrounded = controller.isGrounded;

        if (controller.isGrounded)
        {
            lastGroundedHeight = transform.position.y;
        }

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
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        flashScreenDamage();

        if (HP < 0)
        {
            // Hey I'm dead!
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
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void toggleDoubleJump()
    {
        canDoubleJump = true; // Gives player double jump
    }
}