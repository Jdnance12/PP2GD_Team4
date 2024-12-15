using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool checkpointActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !checkpointActivated)
        {
            checkpointActivated = true;

            // Save checkpoint position and player stats
            Vector3 savePosition = transform.position;
            savePosition.y += 1f; // Store the position 1 unit above

            PlayerPrefs.SetFloat("Checkpoint_X", savePosition.x);
            PlayerPrefs.SetFloat("Checkpoint_Y", savePosition.y);
            PlayerPrefs.SetFloat("Checkpoint_Z", savePosition.z);

            // Update checkpoint-loaded flag
            GameManager.instance.isCheckpointLoaded = true;

            // Save player stats
            GameManager.instance?.SaveGame(other.transform);

            // Update checkpoint button visibility
            buttonFunctions buttonScript = FindObjectOfType<buttonFunctions>();
            if (buttonScript != null)
            {
                buttonScript.UpdateCheckpointButtonState();
            }

            // Show checkpoint UI feedback
            GameManager.instance?.ShowCheckpointUI();
            Debug.Log("Checkpoint activated, position saved.");
        }
    }

}
