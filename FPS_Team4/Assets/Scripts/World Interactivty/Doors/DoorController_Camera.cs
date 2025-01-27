using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController_Camera : MonoBehaviour
{
    public float interactionDistance;
    public GameObject inText;

    public float autoCloseDelay = 5.0f;

    // Start is called before the first frame update
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, interactionDistance))
        {
            //Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            if(hit.collider.gameObject.tag == "door")
            {
                Debug.Log("Door detected: " + hit.collider.gameObject.name);

                // Traverse up the hierarchy to find the animator component regardless of it being on parent or grandparent object relative to door
                Transform currentTransform = hit.collider.transform;
                Animator doorAnim = null;
                Door door = null;

                // find the components
                while (currentTransform != null)
                {
                    doorAnim = currentTransform.GetComponent<Animator>();
                    door = currentTransform.GetComponent<Door>();

                    if (doorAnim != null && door != null)
                    {
                        break;
                    }

                    currentTransform = currentTransform.parent;
                }

                if(doorAnim == null || door == null)
                {
                    Debug.LogWarning("Animator or Door component not found on the door's parent hierarchy.");
                    inText.SetActive(false);
                    return;
                }

                inText.SetActive(true);
                Debug.Log("inText set to active");

                if (Input.GetButton("Interact"))
                {
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(door.doorOpenAnimationName))
                    {
                        Debug.Log("Door is closing");
                        doorAnim.ResetTrigger("open");
                        doorAnim.SetTrigger("close");
                        StopCoroutine(AutoCloseDoor(doorAnim));
                    }
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(door.doorCloseAnimationName))
                    {
                        Debug.Log("Door is opening");
                        doorAnim.ResetTrigger("close");
                        doorAnim.SetTrigger("open");
                        StartCoroutine(AutoCloseDoor(doorAnim));
                    }
                }
            }
            else
            {
                inText.SetActive(false);
                //Debug.Log("inText set to inactive");
            }
        }
        else
        {
            inText.SetActive(false);
            //Debug.Log("inText set to inactive");
        }
    }

    private IEnumerator AutoCloseDoor(Animator doorAnim)
    {
        Debug.Log("AutoCloseDoor coroutine started");
        yield return new WaitForSeconds(autoCloseDelay);
        Door door = doorAnim.GetComponentInParent<Door>();
        if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(door.doorOpenAnimationName))
        {
            Debug.Log("Closing the door automatically");
            doorAnim.ResetTrigger("open");
            doorAnim.SetTrigger("close");
        }
    }
}
