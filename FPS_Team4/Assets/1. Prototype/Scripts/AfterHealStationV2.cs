using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AfterHealStationV2 : MonoBehaviour
{

    [SerializeField] int healAmount;

    [SerializeField] GameObject rampUp;  // Reference to the ramp up
    [SerializeField] GameObject rampDown; // Reference to the ramp down
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject healStation;

    public Image background;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IRecharge rechargeable = other.GetComponent<IRecharge>();
            
            rechargeable.restoreHP(healAmount);
            Debug.Log("Healing station activated. Healing amount: " + healAmount);

            rampUp.SetActive(false);
            rampDown.SetActive(true);
            enemy.SetActive(true);
            healStation.SetActive(true);

            Destroy(gameObject);           
        }
    }
}
