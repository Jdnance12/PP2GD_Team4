using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DoorInteractionPrompt : MonoBehaviour
{
    [SerializeField] public GameObject uiParent; //reference to the text element
    [SerializeField] private SlidingDoor slidingDoor; // Reference to the SlidingDoor script
    [SerializeField] private bool showPopupOnce = true; // flag to control if the popup should show

    float timeScaleOriginal; // Original time scale

    private bool wasTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        uiParent.SetActive(false); // uiParent is off
        Debug.Log("UI Parent set to inactive at Start.");
        timeScaleOriginal = Time.timeScale; // Save original time scale
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasTriggered)
        {
            if (showPopupOnce)
            {
                Debug.Log("Trigger entered by Player. Calling ShowPopup().");
                StartCoroutine(ActivateDoorsPrompt());

                // open the door using the slidingdoor script
                if(slidingDoor != null)
                {
                    slidingDoor.isOpening = true;
                }

                wasTriggered = true;
            }
            else
            {
                // directly handle door opening if popup is not to be shown
                if(slidingDoor != null)
                {
                    slidingDoor.isOpening = true;
                }
            }
        }
    }

    private IEnumerator ActivateDoorsPrompt()
    {
        yield return new WaitForSeconds(0.1f); // Small delay before activation
        ShowPopup();
    }

    public void ShowPopup()
    {
        Debug.Log("ShowPopup called. Activating UI Parent.");
        uiParent.SetActive(true); // show the uiParent
        Debug.Log("UI Parent active status: " + uiParent.activeSelf);

        foreach (Transform child in uiParent.transform)
        {
            Debug.Log("Child " + child.name + " active status before setting: " + child.gameObject.activeSelf);
            child.gameObject.SetActive(true); // Ensure child elements are explicitly set to active
            Debug.Log("Child " + child.name + " active status after setting: " + child.gameObject.activeSelf);

            // Check if TextMeshPro component has a valid font asset
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                Debug.Log("TextMeshPro component found on: " + child.name + ". Font asset: " + textMesh.font);
                if (textMesh.font == null)
                {
                    Debug.LogError("TextMeshPro component on " + child.name + " has no font asset assigned.");
                } 
            }


            // Check grandchildren (Button Text)
            foreach (Transform grandchild in child.transform)
            { 
                Debug.Log("Grandchild " + grandchild.name + " active status before setting: " + grandchild.gameObject.activeSelf);
                grandchild.gameObject.SetActive(true); // Ensure grandchild elements are explicitly set to active
                Debug.Log("Grandchild " + grandchild.name + " active status after setting: " + grandchild.gameObject.activeSelf);
            }
        }

        //Time.timeScale = 0f; // pause the game
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void HidePopup()
    {
        Debug.Log("HidePopup called. Deactivating UI Parent.");
        uiParent.SetActive(false);
        Debug.Log("UI Parent active status: " + uiParent.activeSelf);
        Time.timeScale = timeScaleOriginal; //resume game
        wasTriggered = true; // set to true to make sure the popup doesn't happen again
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnButtonClicked()
    {
        Debug.Log("Button clicked to confirm door info screen was seen.");

        // hide the ui and resume the game
        HidePopup();        
    }
}
