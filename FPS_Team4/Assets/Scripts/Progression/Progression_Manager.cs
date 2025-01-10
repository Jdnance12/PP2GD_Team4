using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Progression_Manager : MonoBehaviour
{

    GameManager gameManager;
    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;
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
