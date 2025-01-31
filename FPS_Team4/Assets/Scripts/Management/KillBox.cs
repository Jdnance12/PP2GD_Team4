// MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // Detect objects entering kill box
    {
        if (other.CompareTag("Player")) // Check if the player entered
        {
            GameManager.instance.YouLose(); // Trigger the loss condition in GameManager
        }
    }
}
