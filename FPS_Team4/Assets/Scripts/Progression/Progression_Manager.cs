using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.VisualScripting;

public class Progression_Manager : MonoBehaviour
{

    GameManager gameManager;

    [Header("---- Bools ----")]
    public bool tutorialActive;
    public bool firstDialogueOpened;
    public bool firstDialogueClosed;
    public bool enemyInPlace;
    public bool gunTutorial;
    public bool firstBossKilled;
    public bool playerLanded;
    public bool bladeTutorial;
    public bool hallenemiesKilled;
    public bool healStationTutorial;
    public bool secondDialogueOpened;
    public bool secondDialogueClosed;
    public bool lastMenu;

    public bool playerInFallPos;

    [Header("---- Floats and Numbers ----")]
    public float explosionForce;

    [Header("---- Tutorial Start Room Game Objects ----")]
    public int enemyCount;
    [SerializeField] GameObject player;
    [SerializeField] public GameObject gunButton;
    [SerializeField] public GameObject bladeButton;
    [SerializeField] GameObject controlBoss;
    [SerializeField] public GameObject controlComputerObj;
    ControlComputer computerScript;
    [SerializeField] GameObject controlDoorObj;
    [SerializeField] public ControlRoomDoor controlDoorScript;
    [SerializeField] public GameObject enemyStartRoom;
    [SerializeField] EnemyBasic enemyScript;
    [SerializeField] GameObject enemyEndPos;
    [SerializeField] GameObject wallWhole;
    [SerializeField] GameObject wallBroken;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Vector3 explosionPos;
    public Coroutine explosionCo;
    public GameObject healStationDoor;
    //[SerializeField] GameObject playerFallPos;
    [SerializeField] public GameObject coreComputerObj;
    CoreComputer coreComputerScript;
    [SerializeField] List<GameObject> enemyList;

    [Header("---- AI Dialogue ----")]
    public float letterDelay;
    private string fullText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;

        gunButton = GameObject.Find("Gun Button");
        bladeButton = GameObject.Find("Blade Button");
        //gunButton.SetActive(true);
        //bladeButton.SetActive(false);

        controlComputerObj = GameObject.Find("Control Computer"); // Finding the Control Computer
        computerScript = controlComputerObj.GetComponent<ControlComputer>(); // Accessing the Control Computers Script

        controlDoorObj = GameObject.Find("Control Door"); // Finding the Control Room Door
        controlDoorScript = controlDoorObj.GetComponent<ControlRoomDoor>(); // Accessing the Control Room Door Script

        enemyScript = enemyStartRoom.GetComponent<EnemyBasic>();

        coreComputerObj = GameObject.Find("Core Computer");
        coreComputerScript = coreComputerObj.GetComponent<CoreComputer>();

        explosionPos = explosionPrefab.transform.position;
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
            gameManager.playerScript.canMove = false;

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
                gameManager.playerScript.canMove = true;
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
        if (enemyInPlace & !gunTutorial)
        {
            gameManager.GamePaused();
            gameManager.menuActive = gameManager.menuMisc;
            gameManager.miscBodyText.text = "This strange AI has taken over one of your guards. " +
                "Open your weapon wheel with E and select your gun to defend yourself. " +
                "Once selected pressing the left mouse button while fire your weapon. " +
                "Select the gun option again to put it away.";
            gameManager.menuActive.SetActive(true);
            gunTutorial = true;

        }
        // Killing the Enemy triggers the explosion
        if (enemyStartRoom == null && !firstBossKilled)
        {
            gameManager.playerScript.canMove = false;

            wallWhole.SetActive(false);
            wallBroken.SetActive(true);
            CharacterController controller = player.GetComponent<CharacterController>();

            Vector3 direction = new Vector3(0, 0, player.transform.position.z - explosionPos.z).normalized;
            explosionCo = StartCoroutine(ApplyExplosionForce(controller, direction, explosionForce));
            
        }
        // When the player lands triggers misc. menu for the Blade Tutorial
        if(playerLanded == true && bladeTutorial == false)
        {
            gameManager.GamePaused();
            gameManager.menuActive = gameManager.menuMisc;
            gameManager.miscBodyText.text = "A quick diagnostic scan shows your heavily damaged and in need of parts to repair yourself. " +
                "Which means you've lost access to your gun. It seems right now the only weapon you have available to you is your blade. " +
                "Press E and select it. You have enemies in the area searching for you.";
            gameManager.menuActive.SetActive(true);
            bladeTutorial = true;
        }
        // When all hall enemies are killed trigger misc. menu to show heal station tutorial
        if(hallenemiesKilled == true && healStationTutorial == false)
        {
            healStationDoor.GetComponent<BoxCollider>().enabled = true;
            gameManager.GamePaused();
            gameManager.menuActive = gameManager.menuMisc;
            gameManager.miscBodyText.text = "All enemies are killed. We need to heal. You've probably already seen it but the doors that are green indicate a player station. In these rooms you'll have a heal station and a repair station." +
                "The heal station heals you to max health. The repair station allows you to upgrade your current weapons and fix any broken tools you have.";
            gameManager.menuActive.SetActive(true);
            healStationTutorial = true;
            healStationDoor.GetComponent<BoxCollider>().enabled = false;
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
                //tutorialActive = false;

                foreach (GameObject enemy in enemyList)
                {
                    enemy.gameObject.SetActive(true);
                }
                secondDialogueClosed = true;
            }
        }

        //When all enemies are killed open last menu
        if(secondDialogueClosed == true && lastMenu == false)
        {
            gameManager.menuActive = gameManager.menuMisc;
            gameManager.miscBodyText.text = "The AI has taken over every thing and has shut me out of the system. Only way I'm going to fix this now is by getting my captians access keys to wipe the system. I'm sure they've been corrupted to.... Let's do this.";
            gameManager.menuActive.SetActive(true);
            gameManager.GamePaused();
            lastMenu = true;
            tutorialActive = false;
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

    public IEnumerator ApplyExplosionForce(CharacterController controller, Vector3 direction, float force)
    {
        float time = 0.05f;
        while (time > 0)
        {
            controller.Move(direction * force * Time.deltaTime);
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
