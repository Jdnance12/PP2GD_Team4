using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test Trigger entered by: " + other.tag + " | Object: " + other.name);
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Test Trigger exited by: " + other.tag + " | Object: " + other.name);
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("Test Trigger stay detected by: " + other.tag + " | Object: " + other.name);
    //}
}
