using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController1 : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 5)] float speed;
    [SerializeField][Range(2, 5)] float sprintMod;

    [Header("Gun Stats")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
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

        sprint();

        moveDir = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * speed * Time.deltaTime);

        controller.Move(playerVel * Time.deltaTime);
    }

    

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = !isSprinting;
            speed = (isSprinting ? speed *= sprintMod : speed /= sprintMod);
        }
    }
}