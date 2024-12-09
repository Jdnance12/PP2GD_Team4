using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController playerController;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform cameraTransform;

    [Header("----- Stats -----")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 5)] int speed;
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(5, 20)] int jumpSpeed;
    [SerializeField][Range(15, 40)] int gravity;
    [SerializeField][Range(1, 5)] int crouchSpeed;
    public Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 normalScale;

    [Header("----- Weapon Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("----- Vectors -----")]
    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    [Header("----- Bools -----")]
    bool isSprinting;
    bool isShooting;
    bool isCrouching;


    // Start is called before the first frame update
    void Start()
    {
        normalScale = transform.localScale;

        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red);

        playerMovement();
        sprint();

    }

    void playerMovement()
    {
        if (playerController.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        //Player Movement
        moveDir = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        playerController.Move(moveDir * speed * Time.deltaTime);

        //Player Rotation

        //Jump
        jump();
        playerController.Move(playerVel * Time.deltaTime);

        //Gravity
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot());
        }

    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void ToggleCrouch()
    {
        if(Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;

            if(isCrouching)
            {

            }
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

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
        StartCoroutine(flashScreen());

        if (HP < 0)
        {
            // Hey I'm dead!
            GameManager.instance.youLose();
        }
    }

    IEnumerator flashScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
}
