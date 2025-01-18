using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ControlRoomDoor : MonoBehaviour
{
    GameManager gameManager;

    [Header("---- Bools ----")]
    public bool isLocked = false;
    public bool isBroken = false;
    public bool isOpening = false;
    public bool playerInRange = false;

    [Header("---- GameObjects ----")]
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    Renderer leftDoorMat;
    Renderer rightDoorMat;

    [Header("---- Positions ----")]
    [SerializeField] Transform closedPositionLeft;
    [SerializeField] Transform closedPositionRight;
    [SerializeField] Transform openPositionLeft;
    [SerializeField] Transform openPositionRight;

    [Header("---- Colors ----")]
    private Color unlockedColor;
    private Color lockedColor;
    private Color brokenColor;

    public float speed = 0.25f;
    private float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        leftDoorMat = leftDoor.GetComponent<Renderer>();
        unlockedColor = new Color(0, 0, 1);
        lockedColor = new Color(1, 0, 0);
        brokenColor = new Color(0, 0, 0);

        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    public void OpenDoor()
    {
        t += Time.deltaTime * speed;
        leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, openPositionLeft.position, t);
        rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, openPositionRight.position, t);
        
    }
    public void CloseDoor()
    {
        t += Time.deltaTime * speed;
        if (!isOpening)
        {
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, closedPositionLeft.position, t);
            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, closedPositionRight.position, t);
        }
    }
}
