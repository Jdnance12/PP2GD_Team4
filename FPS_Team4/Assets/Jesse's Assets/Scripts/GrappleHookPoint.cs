using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHookPoint : MonoBehaviour
{
    private gameManager gm;
    [SerializeField] GameObject uiElement;
    [SerializeField] Transform playerTrans;
    
    [SerializeField] float angleFOV;

    public bool playerInRange;

    private void Start()
    {
        gm = gameManager.instance;
        playerTrans = gm.player.transform;
    }

    private void Update()
    {
        if(playerInRange)
        {
            uiElement.SetActive(true);
            FacePlayer();
        }
        else
        {
            uiElement.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void FacePlayer()
    {
        Vector3 directionToPlayer = playerTrans.position - uiElement.transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        uiElement.transform.rotation = playerTrans.rotation;
    }
}
