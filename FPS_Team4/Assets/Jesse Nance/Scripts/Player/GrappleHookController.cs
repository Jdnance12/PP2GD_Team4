using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHookController : MonoBehaviour
{
    private GameManager gm;

    public GameObject player;
    public PlayerController playerCtrlr;

    public GameObject upgradeObject;
    public UpgradeScript upgradeScript;

    public GameObject playerCam;
    public Transform playerCamTrans;
    public Camera_Controller camController;

    public Transform grappleStart;

    public GameObject heavyObject;
    public Transform hookPoint;
    public LayerMask grappleLayer;
    public LineRenderer lineRenderer;

    public float maxDistance;
    public float hookSpeed;
    public float pullSpeed;
    private float originalGravity;
    private float originalHookSpeed;

    public bool isGrappling = false;
    public bool pullingObject = false;
    public bool drawLine = false;

    public Vector3 grappleTarget;

    private void Start()
    {
        gm = GameManager.instance;
        upgradeObject = GameObject.Find("Game Manager");
        upgradeScript = upgradeObject.GetComponent<UpgradeScript>();

        player = gm.player;
        playerCtrlr = gm.playerScript;

        playerCam = Camera.main.gameObject;
        playerCamTrans = playerCam.transform;
        camController = playerCam.GetComponent<Camera_Controller>();

        lineRenderer = player.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        originalGravity = playerCtrlr.gravity;
    }

    private void Update()
    {

        //maxDistance = upgradeScript.upgradedHookDistance;

        if (Input.GetButton("Grapple"))
        {
            LaunchHook();
            camController.enabled = false;

            if (isGrappling)
            {
                Grapple();
                playerCtrlr.gravity = 0f;
            }
            else if (pullingObject && heavyObject != null)
            {
                PullObject();
            }
        }

        if (Input.GetButtonUp("Grapple"))
        {
            isGrappling = false;
            pullingObject = false;
            drawLine = false;
            lineRenderer.positionCount = 0;
            playerCtrlr.gravity = originalGravity;
            camController.enabled = true;
        }

        UpdateLine();
    }

    void LaunchHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamTrans.position, playerCamTrans.forward, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Heavy Object") || hit.collider.CompareTag("Enemy"))
            {
                pullingObject = true;
                heavyObject = hit.collider.gameObject;
                lineRenderer.positionCount = 2;
            }
            else if(hit.collider.CompareTag("Grapple Point"))
            {
                grappleTarget = hit.point;
                isGrappling = true;
                lineRenderer.positionCount = 2;
                camController.enabled = false;

            }
        }
    }

    void Grapple()
    {
        transform.position = Vector3.MoveTowards(transform.position, grappleTarget, hookSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, grappleTarget) < 1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            isGrappling = false;
            lineRenderer.positionCount = 0;
        }
    }

    void PullObject()
    {
        float yPos = heavyObject.transform.position.y;

        Vector3 targetPosition = new Vector3(transform.position.x, yPos, transform.position.z);
        heavyObject.transform.position = Vector3.MoveTowards(heavyObject.transform.position, targetPosition, pullSpeed * Time.deltaTime);
        if (Vector3.Distance(heavyObject.transform.position, targetPosition) < 1f)
        {
            pullingObject = false;
            lineRenderer.positionCount = 0;
        }
    }

    void UpdateLine()
    {
        if (lineRenderer.positionCount > 0)
        {
            lineRenderer.SetPosition(0, grappleStart.position);
            if (pullingObject && heavyObject != null)
            {
                lineRenderer.SetPosition(1, heavyObject.transform.position);
            }
            else
            {
                lineRenderer.SetPosition(1, grappleTarget);
            }
        }
    }
}
