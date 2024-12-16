using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpActivator : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerController playerCtrlr = other.GetComponent<playerController>();
            if(playerCtrlr != null )
            {
                playerCtrlr.canDoubleJump = true;
            }

            Destroy(gameObject);
        }
    }
}
