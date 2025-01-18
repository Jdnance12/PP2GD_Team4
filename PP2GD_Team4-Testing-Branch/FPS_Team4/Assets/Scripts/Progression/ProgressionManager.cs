using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    [Header("---- Game Objects ----")]
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject player;

    [SerializeField] GameObject playerStart;
    [SerializeField] GameObject enemyTransform;

    [SerializeField] GameObject initialDamager;
    [SerializeField] GameObject initialWeaponTutorial;
    [SerializeField] GameObject initialEnemy;
    [SerializeField] GameObject initalPlayerStation;

    [SerializeField] bool tutoralMenu;
    [SerializeField] bool weaponTutorial;
    [SerializeField] bool enemyDestroyed;
    [SerializeField] bool playerStation;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;
        playerStart = GameObject.Find("Player_Start");

        player.transform.position = playerStart.transform.position;
        initialEnemy.SetActive(false);
        initialWeaponTutorial.SetActive(false);

        gm.menuActive = gm.menuStart;
        gm.menuActive.SetActive(true);
        gm.GamePaused();
    }

    // Update is called once per frame
    void Update()
    {
        TutorialQuest();
    }
    public void TutorialQuest()
    {
        if(initialDamager == null && !tutoralMenu)
        {
            tutoralMenu = true;
            gm.menuActive = gm.menuMisc;
            gm.menuMisc.SetActive(true);
            gm.miscBodyText.text = "You've been damaged after a virus has taken over the facility. Take up arms and restore the system. For now, let's find a station to heal.";
            initialEnemy.SetActive(true);
            initialWeaponTutorial.SetActive(true);
            gm.GamePaused();
        }
        if(initialWeaponTutorial == null && !weaponTutorial)
        {
            weaponTutorial = true;
            gm.menuActive = gm.menuMisc;
            gm.menuMisc.SetActive(true);
            gm.miscBodyText.text = "Watch out! There's an enemy nearby!" +
                "You can equip your weapons and skills by pressing E or Q." +
                "Use your mouse to select which weapon you want to use.";
            gm.GamePaused();
        }

        if(initialEnemy == null && !enemyDestroyed)
        {
            enemyDestroyed = true;
            gm.menuActive = gm.menuMisc;
            gm.menuMisc.SetActive(true);
            gm.miscBodyText.text = "You did it! Good job" +
                "He dropped something, go and collect it!" +
                "You'll be able to use this to for upgrades.";
            gm.GamePaused();
        }
        if(initalPlayerStation == null && !playerStation)
        {
            playerStation = true;
            gm.menuActive = gm.menuMisc;
            gm.menuMisc.SetActive(true);
            gm.miscBodyText.text = "Player Stations have two functions. One is for healing and the other is for UPGRADES. Step on the white circle to heal yourself. Walk up to the blue section and press T to access the upgrade menu.";
            gm.GamePaused();
        }
    }
}
