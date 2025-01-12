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
    public bool firstDialoguePlayed;

    [Header("---- Game Objects ----")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject controlBoss;

    public float letterDelay;
    private string fullText;

    public GameObject controlComputer;
    ControlComputer computerScript;




    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;

        controlComputer = GameObject.Find("Control Computer");
        computerScript = controlComputer.GetComponent<ControlComputer>();
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
        if(computerScript.playerInteracted == true && !firstDialoguePlayed)
        {
            computerScript.aiFace.SetActive(true);

            gameManager.GamePaused();
            gameManager.menuActive = gameManager.aiDialogueTextImage;
            gameManager.menuActive.SetActive(true);

            fullText = "What's this? How are you... Oh this won't do. DESTROY THE MACHINE!";
            StopCoroutine(ShowText(gameManager.aiDialogueText));
            StartCoroutine(ShowText(gameManager.aiDialogueText));
            firstDialoguePlayed = true;
        }
    }
    public void GameProgression()
    {

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
