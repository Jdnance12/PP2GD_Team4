using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHealStation : MonoBehaviour
{

    public Image background;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;


    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();

        background.color = Color.blue;
        titleText.text = "Healing Stations";
        bodyText.text = "Standing on the white plateform will heal you to full health when you need it.";

        Destroy(gameObject);
    }
}
