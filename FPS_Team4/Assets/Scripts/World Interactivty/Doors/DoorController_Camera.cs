using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController_Camera : MonoBehaviour
{
    public float interactionDistance;
    public GameObject inText;
    public string doorOpenAnimationName, doorCloseAnimationName;

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
                //Debug.Log("Door detected: " + hit.collider.gameObject.name);

                GameObject doorParent = hit.collider.transform.parent.parent.gameObject;
                //Debug.Log("Door parent: " + doorParent.name);
                Animator doorAnim = doorParent.GetComponent<Animator>();
                if(doorAnim == null)
                {
                    //Debug.LogWarning("No Animator component found on the door parent");
                    inText.SetActive(false);
                    return;
                }
                inText.SetActive(true);
                //Debug.Log("inText set to active");
                if(Input.GetButton("Interact"))
                {
                    if(doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimationName))
                    {
                        doorAnim.ResetTrigger("open");
                        doorAnim.SetTrigger("close");
                    }
                    if(doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimationName))
                    {
                        doorAnim.ResetTrigger("close");
                        doorAnim.SetTrigger("open");
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
}
