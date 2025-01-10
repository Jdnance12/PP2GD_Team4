using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlRoomDoor : MonoBehaviour
{
    GameManager gameManager;

    [Header("---- Bools ----")]
    public bool isLocked = false;
    public bool isBroken = false;
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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        leftDoorMat = leftDoor.GetComponent<Renderer>();
        unlockedColor = new Color(0, 1, 0);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
