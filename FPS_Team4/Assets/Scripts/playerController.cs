using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField] [Range(1, 5)] float speed;
    [SerializeField] [Range(2, 5)] float sprintMod;
    [SerializeField] float crouchHeight;
    [SerializeField] [Range(2, 5)] float crouchMod;
    [SerializeField] [Range(1, 3)] float jumpMax;
    [SerializeField] [Range(5, 20)] float jumpSpeed;
    [SerializeField] [Range(15, 40)] float gravity;

    [Header("Gun Stats")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && !isCrouching)
        {
            isJumping = true;
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

        if (HP < 0)
        {
            // Hey I'm dead!
            GameManager.instance.youLose();
        }
    }
}