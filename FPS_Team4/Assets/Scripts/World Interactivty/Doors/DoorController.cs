using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorParent; // reference door parent
    private Animator animator;
    private bool isOpen = false;
    private bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = doorParent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButton("Interact"))
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if(isOpen)
        {
            animator.SetTrigger("open");
        }
        else
        {
            animator.SetTrigger("close");
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

}
