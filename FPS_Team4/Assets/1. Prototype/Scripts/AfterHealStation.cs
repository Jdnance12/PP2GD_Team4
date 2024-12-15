using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AfterHealStation : MonoBehaviour
{
    public float healAmount;

    public GameObject rampUp;
    public GameObject rampDown;
    public GameObject enemy;

    public TextMeshProUGUI uiTitleText;
    public TextMeshProUGUI uiBodyText;

    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        playerInRange = false;
    }

    private void Update()
    {
        HealPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }

    void HealPlayer()
    {
        if(playerInRange && Input.GetButtonDown("Interact"))
        {
            rampUp.SetActive(false);
            rampDown.SetActive(true);
            enemy.SetActive(true);

            GameManager.instance.DialogueScreen();
            uiTitleText.text = "Watch Out!";
            uiBodyText.text = "You've been healed but watch out!";

            Destroy(gameObject);
        }
    }

}
