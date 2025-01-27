using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public string doorOpenAnimationName;
    public string doorCloseAnimationName;
    [SerializeField] public bool isLocked = false;
    public Text lockedStatus;

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked");

        // Hide the lockedStatus UI element
        if (lockedStatus != null)
        {
            lockedStatus.text = "Door is Unlocked!";
        }
        else
        {
            Debug.LogWarning("lockedStatus is not assigned in the Inspector.");
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}
