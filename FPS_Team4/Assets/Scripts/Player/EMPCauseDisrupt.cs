using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPCauseDisrupt : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDisrupt disruptable = other.GetComponent<IDisrupt>();
        if(disruptable != null )
        {
            disruptable.causeDisrupt();
        }
    }
}
