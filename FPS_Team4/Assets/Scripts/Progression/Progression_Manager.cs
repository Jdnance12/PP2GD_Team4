using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Progression_Manager : MonoBehaviour
{

    GameManager gameManager;

    [Header("---- Bools ----")]
    public bool tutorialActive;
    public bool firstDialogueOpened;
    public bool firstDialogueClosed;
    public bool enemyInPlace;
    public bool firstBossKilled;
    public bool secondDialogueOpened;
    public bool secondDialogueClosed;

    [Header("---- Tutorial Start Room Game Objects ----")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject controlBoss;
    [SerializeField] public GameObject controlComputerObj;
    ControlComputer computerScript;
    [SerializeField] GameObject controlDoorObj;
    [SerializeField] public ControlRoomDoor controlDoorScript;
    [SerializeField] public GameObject enemyStartRoom;
    [SerializeField] GameObject enemyEndPos;
    [SerializeField] public GameObject coreComputerObj;
    CoreComputer coreComputerScript;

    [Header("---- AI Dialogue ----")]
    public float letterDelay;
    private string fullText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;

        controlComputerObj = GameObject.Find("Control Computer"); // Finding the Control Computer
        computerScript = controlComputerObj.GetComponent<ControlComputer>(); // Accessing the Control Computers Script

        controlDoorObj = GameObject.Find("Control Door"); // Finding the Control Room Door
        controlDoorScript = controlDoorObj.GetComponent<ControlRoomDoor>(); // Accessing the Control Room Door Script

        coreComputerObj = GameObject.Find("Core Computer");
        coreComputerScript = coreComputerObj.GetComponent<CoreComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialActive)
        {
            Tutorial();
        }
        else
        {
            GameProgression();
        }
    }


    public void Tutorial()
    {
        // Accessing the Computer
        if (computerScript.playerInteracted == true && !firstDialogueOpened)
        {
            computerScript.aiFace.SetActive(true);

            //gameManager.GamePaused();
            gameManager.menuActive = gameManager.aiDialogueTextImage;
            gameManager.menuActive.SetActive(true);

            fullText = "What's this? How are you... Oh this won't do. DESTROY THE MACHINE!";
            StopCoroutine(ShowText(gameManager.aiDialogueText));
            StartCoroutine(ShowText(gameManager.aiDialogueText));
            firstDialogueOpened = true;
        }
        //Closes the Diagolgue for the AI
        if(firstDialogueOpened == true && firstDialogueClosed == false)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                gameManager.menuActive.SetActive(false);
                gameManager.menuActive = null;
                firstDialogueClosed = true;
            }
        }
        //Opens the Door
        if(firstDialogueClosed)
        {
            controlDoorScript.OpenDoor();
        }
        //When the door opens the enemy activates
        if (firstDialogueClosed && !enemyInPlace)
        {
            enemyStartRoom.SetActive(true);
            enemyStartRoom.transform.position = Vector3.Lerp(enemyStartRoom.transform.position, enemyEndPos.transform.position, Time.deltaTime * 2f);
            
        }
        if (enemyStartRoom == null && !firstBossKilled)
        {
            gameManager.GamePaused();
            gameManager.menuActive = gameManager.menuMisc;
            gameManager.miscBodyText.text = "That was tough. I'm hurt but I need to focus on getting to the system core first and see what's going on";
            gameManager.menuActive.SetActive(true);
            firstBossKilled = true;
        }
        if(coreComputerScript.playerInteracted && !secondDialogueOpened)
        {
            coreComputerScript.aiFace.SetActive(true);

            gameManager.menuActive = gameManager.aiDialogueTextImage;
            gameManager.menuActive.SetActive(true);

            fullText = "There's nothing that you can do to stop me! You will die here.";
            StopCoroutine(ShowText(gameManager.aiDialogueText));
            StartCoroutine(ShowText(gameManager.aiDialogueText));

            secondDialogueOpened = true;
        }
        if (secondDialogueOpened && !secondDialogueClosed)
        {
            if(Input.GetKey(KeyCode.Tab))
            {
                gameManager.menuActive.SetActive(false);
                gameManager.menuActive = null;
                secondDialogueClosed = true;
                tutorialActive = false;
            }
        }
    }
    public void GameProgression()
    {
        if(coreComputerScript.playerInRange && gameManager.nodeCount == 3)
        {
            if (Input.GetButton("Interact"))
            {
                gameManager.YouWin();
            }
        }
    }
    IEnumerator ShowText(TMP_Text textObj)
    {
        textObj.text = "";

        foreach(char letter in fullText.ToCharArray())
        {
            textObj.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        
    }
}
