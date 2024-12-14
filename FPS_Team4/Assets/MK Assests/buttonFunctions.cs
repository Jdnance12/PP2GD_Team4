using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] private GameObject welcomeScreen; // Welcome screen reference
    [SerializeField] private GameObject reticle; // Reticle reference
    [SerializeField] private GameObject playerHP; // Player HP bar reference
    [SerializeField] private GameObject nodesCollectionLabel; // Nodes collection label reference

    // void Start()
    // {
    //     // Game starts in a paused state and shows the welcome screen
    //     if (GameManager.instance != null)
    //     {
    //         GameManager.instance.statePause();
    //     }

    //     // Only the welcome screen is active at the start
    //     welcomeScreen?.SetActive(true);
    //     reticle?.SetActive(false);
    //     playerHP?.SetActive(false);
    //     nodesCollectionLabel?.SetActive(false);
    // }

    public void Play()
    {
        // Disable the welcome screen
        if (welcomeScreen != null)
        {
            welcomeScreen.SetActive(false);
        }

        // Enable gameplay-related UI elements
        if (reticle != null)
        {
            reticle.SetActive(true);
        }

        if (playerHP != null)
        {
            playerHP.SetActive(true);
        }

        if (nodesCollectionLabel != null)
        {
            nodesCollectionLabel.SetActive(true);
        }

        // Unpause the game
        if (GameManager.instance != null)
        {
            GameManager.instance.stateUnpause();
        }
    }

    public void Resume()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.stateUnpause();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (GameManager.instance != null)
        {
            GameManager.instance.stateUnpause();
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
