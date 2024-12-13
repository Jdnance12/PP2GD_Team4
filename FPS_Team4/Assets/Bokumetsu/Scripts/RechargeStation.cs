using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeStation : MonoBehaviour
{
    [SerializeField] int healAmount;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            IRecharge rechargeable = other.GetComponent<IRecharge>();
            if(rechargeable != null )
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
