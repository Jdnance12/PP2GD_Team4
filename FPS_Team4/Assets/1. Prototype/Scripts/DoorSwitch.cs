using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{

    public GameObject workingDoor;
    public GameObject brokenDoor;

    private void OnTriggerEnter(Collider other)
    {
        workingDoor.SetActive(false);
        brokenDoor.SetActive(true);
        Destroy(gameObject);
    }
}
