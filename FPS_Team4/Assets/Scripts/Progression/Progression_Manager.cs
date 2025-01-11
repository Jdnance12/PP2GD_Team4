using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Progression_Manager : MonoBehaviour
{

    GameManager gameManager;
    [Header("---- Game Objects ----")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject controlBoss;

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
        GameProgression();
    }
    public void GameProgression()
    {

    }
}
