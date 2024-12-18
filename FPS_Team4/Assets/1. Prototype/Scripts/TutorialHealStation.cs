using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHealStation : MonoBehaviour
{
    [SerializeField] int healAmount;

    [SerializeField] GameObject rampUp;  // Reference to the ramp up
    [SerializeField] GameObject rampDown; // Reference to the ramp down
    [SerializeField] GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.statePause();
        GameManager.instance.DialogueScreen();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IRecharge rechargeable = other.GetComponent<IRecharge>();
            if (rechargeable != null)
            {
                rechargeable.restoreHP(healAmount);
                Debug.Log("Healing station activated. Healing amount: " + healAmount);

                
            }
            else
            {
                Debug.LogWarning("Player object does not implement IRecharge interface.");
            }
        }
    }
}
