using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//MK Added - Start
using TMPro; // Text Mesh Pro to support Warning Text for no data found
using UnityEngine.UI; // Required for CanvasScaler for no data found
//MK Added - End

public class ButtonFunctions : MonoBehaviour
{   
    public void Resume()
    {
        if (!GameManager.instance.isGameOver) //MK Added - Prevents Resume during game over
        {
            GameManager.instance.GameUnPaused();
            //MK Added - Start
            DestroyWarning(); // Immediately hides warning text
            //MK Added - End
        }
    }
    public void Restart()
    {
        GameManager.instance.RestartGame(); //MK Added - Uses RestartGame function
        GameManager.instance.GameUnPaused();
        //MK Added - Start
        DestroyWarning(); // Immediately hides warning text
        //MK Added - End
    }
    public void PlayButton()
    {
        if (!GameManager.instance.isGameOver) //MK Added - Prevents Play during game over
        {
            GameManager.instance.menuActive.SetActive(false);
            GameManager.instance.GameUnPaused();
            //MK Added - Start
            DestroyWarning(); // Immediately hides warning text
            //MK Added - End
            GameManager.instance.menuActive = null;
        }
    }
    //MK Added - Start
    private GameObject dynamicWarningObject; // Tracks warning text object
    public void LoadButton()
    {
        if (SaveSystem.DoesSaveFileExist()) // Check if save file exists
        {
            GameManager.instance.LoadGame(); // Load game
        }
        else
        {
            ShowDynamicWarning("No Save Data Found"); // Show No Save Data Found warning
        }
    }

    private void ShowDynamicWarning(string message) // Displays warning dynamically
    {
        if (dynamicWarningObject != null) Destroy(dynamicWarningObject); // Removes existing warning if active

        dynamicWarningObject = new GameObject("DynamicWarningText"); // Creates new warning object
        TMP_Text warningText = dynamicWarningObject.AddComponent<TextMeshProUGUI>(); // Adds TextMeshPro

        warningText.text = message; // Sets warning message text
        warningText.fontSize = 36; // Adjusts font size for visibility
        warningText.color = Color.red; // Changes text color to red
        warningText.alignment = TextAlignmentOptions.Center; // Aligns text in center

        GameObject canvas = GameObject.Find("UI"); // Finds main UI canvas
        if (canvas != null)
        {
            dynamicWarningObject.transform.SetParent(canvas.transform, false); // Attaches warning to canvas
        }
        else
        {
            Debug.LogError("UI Canvas not found"); // Logs error if canvas is missing
            return; // Exits if canvas is missing
        }

        RectTransform rectTransform = dynamicWarningObject.GetComponent<RectTransform>(); // Gets RectTransform
        rectTransform.anchoredPosition = new Vector2(0, -50); // Sets warning position on screen
        rectTransform.sizeDelta = new Vector2(600, 100); // Adjusts text box size
    }

    private void DestroyWarning() // Destroys active warning object
    {
        if (dynamicWarningObject != null) Destroy(dynamicWarningObject); // Removes warning if present
    }

    public void ShowDynamicSuccess(string message) // Displays success message dynamically
    {
        if (dynamicWarningObject != null) Destroy(dynamicWarningObject); // Removes existing warning if active

        dynamicWarningObject = new GameObject("DynamicSuccessText"); // Creates new success object
        TMP_Text successText = dynamicWarningObject.AddComponent<TextMeshProUGUI>(); // Adds TextMeshPro

        successText.text = message; // Sets success message text
        successText.fontSize = 36; // Adjusts font size for visibility
        successText.color = Color.green; // Changes text color to green
        successText.alignment = TextAlignmentOptions.Center; // Aligns text in center

        GameObject canvas = GameObject.Find("UI"); // Finds main UI canvas
        if (canvas != null)
        {
            dynamicWarningObject.transform.SetParent(canvas.transform, false); // Attaches success text to canvas
        }
        else
        {
            Debug.LogError("UI Canvas not found"); // Logs error if canvas is missing
            return; // Exits if canvas is missing
        }

        RectTransform rectTransform = dynamicWarningObject.GetComponent<RectTransform>(); // Gets RectTransform
        rectTransform.anchoredPosition = new Vector2(0, -50); // Sets success message position on screen
        rectTransform.sizeDelta = new Vector2(600, 100); // Adjusts text box size

        StartCoroutine(DestroySuccessAfterDelay(3f)); // Automatically destroy success message after 3 seconds
    }

    private IEnumerator DestroySuccessAfterDelay(float delay) // Destroys success message after delay
    {
        yield return new WaitForSecondsRealtime(delay); // Waits for specified time
        if (dynamicWarningObject != null)
        {
            Destroy(dynamicWarningObject); // Destroys success object
        }
    }
    //MK Added - End

        public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
