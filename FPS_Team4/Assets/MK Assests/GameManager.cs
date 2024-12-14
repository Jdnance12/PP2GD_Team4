using UnityEngine; // Unity engine base
using System.Collections; // Collection management
using System.Collections.Generic; // Advanced collections
using TMPro; // TextMeshPro for UI
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen; // Reference to StartScreen UI
    public static GameManager instance; // Singleton reference

    public GameObject drone; // Drone reference
    public GameObject inspectionPointPrefab;
    [SerializeField] GameObject menuActive; // Current active menu
    [SerializeField] GameObject menuPause; // Pause menu object
    [SerializeField] GameObject menuWin, menuLose; // Win/Lose menus
    [SerializeField] private GameObject welcomeScreen; // Welcome screen UI


    [SerializeField] TMP_Text activeThreatsText; // Active threats display
    public TMP_Text ActiveThreatsText
    {
        get => activeThreatsText;
        set
        {
            activeThreatsText = value;
            UpdateThreatDisplay(); // Automatically update UI when changed
        }
    }

    [SerializeField] TMP_Text nodesCollectedText; // Node count display
    public TMP_Text NodesCollectedText
    {
        get => nodesCollectedText;
        set
        {
            nodesCollectedText = value;
            UpdateNodeCollection(); // Automatically update UI when changed
        }
    }

    [SerializeField] TMP_Text nodeInteractionText; // UI for node interaction prompt
    public TMP_Text NodeInteractionText
    {
        get => nodeInteractionText;
        set
        {
            nodeInteractionText = value;
            if (nodeInteractionText != null)
            {
                nodeInteractionText.gameObject.SetActive(false); // Ensure it's hidden initially
            }
        }
    }

    [SerializeField] TMP_Text interactWithComputerYesText; // UI for successful interaction
    [SerializeField] TMP_Text interactWithComputerNoText; // UI for missing nodes

    public GameObject computer; // Reference to the computer object

    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject player; // Player object reference
    public playerController playerScript; // Player script reference

    public bool isPaused; // Pause state flag

    float timeScaleOriginal; // Original time scale
    int totalNodes; // Total nodes in the level
    int collectedNodes; // Number of collected nodes
    int activeThreats; // Total active threats drones + soldiers
    public bool allNodesCollected = false; // Flag to check if all nodes are collected

    public bool AllNodesCollected => allNodesCollected; // Public property

    void Awake() // Initial setup
    {
        instance = this; // Set singleton instance
        timeScaleOriginal = Time.timeScale; // Save original time scale
        player = GameObject.FindWithTag("Player"); // Find player by tag
        playerScript = player.GetComponent<playerController>(); // Get player script
        playerHPBar.color = Color.green;
        
        totalNodes = GameObject.FindGameObjectsWithTag("Node").Length; // Find all nodes
        collectedNodes = 0; // Initialize collected nodes
        activeThreats = 0; // Initialize active threats

        // Ensure UI elements are initially set to the correct state
        if (interactWithComputerYesText != null)
        {
            interactWithComputerYesText.gameObject.SetActive(false); // Hide by default
        }

        if (interactWithComputerNoText != null)
        {
            interactWithComputerNoText.gameObject.SetActive(false); // Hide by default
        }

        if (nodeInteractionText != null)
        {
            nodeInteractionText.gameObject.SetActive(false); // Ensure node interaction UI is hidden
        }

        UpdateThreatDisplay(); // Initialize UI for threats
        UpdateNodeCollection(); // Initialize UI for nodes

        //Automatically find the StartScreen GameObject in the scene
        if (startScreen == null)
        {
            startScreen = GameObject.Find("StartScreen"); // Search for StartScreen by name
            if (startScreen == null)
            {
                Debug.LogError("StartScreen not found in the scene! Please ensure it exists.");
            }
        }
    }

    void Update() // Runs every frame
    {
        if (Input.GetButtonDown("Cancel")) // Check for pause/unpause input
        {
            if (menuActive == null) // No active menu
            {
                statePause(); // Enter pause state
                menuActive = menuPause; // Set pause menu
                menuActive.SetActive(true); // Show menu
            }
            else if (menuActive == menuPause) // Pause menu active
            {
                stateUnpause(); // Unpause game
            }
        }
    }

    public void RegisterThreat() // Called by enemies when they spawn
    {
        activeThreats++; // Increment active threats
        UpdateThreatDisplay();
    }

    public void UnregisterThreat() // Called by enemies when they die or deactivate
    {
        activeThreats = Mathf.Max(0, activeThreats - 1); // Decrement threats (never below 0)
        UpdateThreatDisplay();
    }

    public void OnStunBegin() // Called when an enemy or drone is stunned
    {
        activeThreats = Mathf.Max(0, activeThreats - 1); // Decrement active threats
        UpdateThreatDisplay();
    }

    public void OnStunEnd() // Called when an enemy or drone recovers from stun
    {
        activeThreats++; // Increment active threats
        UpdateThreatDisplay();
    }

    public void NodeCollected() // Called when a node is collected
    {
        collectedNodes++; // Increment collected nodes
        UpdateNodeCollection();

        // Hide node interaction UI when the node is collected
        HideNodeInteractionUI();

        if (collectedNodes >= totalNodes) // Check if all nodes are collected
        {
            allNodesCollected = true; // Flag that all nodes are collected
            ActivateComputer(); // Enable computer interaction
        }
    }

    void UpdateThreatDisplay() // Update the active threats UI
    {
        if (activeThreatsText != null)
        {
            activeThreatsText.text = $"{activeThreats:D3}"; // Update UI
        }
    }

    void UpdateNodeCollection() // Update node collection UI
    {
        if (nodesCollectedText != null)
        {
            nodesCollectedText.text = $"{collectedNodes}/{totalNodes}"; // Update UI
        }
    }

    void ActivateComputer() // Enable computer interaction
    {
        if (computer != null)
        {
            computer.GetComponent<Computer>().EnableInteraction(); // Allow interaction with the computer
        }
        else
        {
            Debug.LogWarning("Computer object is not assigned in the GameManager!");
        }
    }

    public void TryWinGame() // Called when interacting with the computer
    {
        if (allNodesCollected) // Ensure all nodes are collected
        {
            WinGame(); // Trigger win condition
        }
        else
        {
            Debug.Log("Cannot interact with the computer until all nodes are collected.");
        }
    }

    void WinGame() // Handle win state
    {
        statePause(); // Pause game
        menuActive = menuWin; // Set win menu
        menuActive.SetActive(true); // Show menu
    }

    public void statePause() // Pause game logic
    {
        isPaused = true; // Set pause state
        Time.timeScale = 0; // Freeze time
        Cursor.visible = true; // Show cursor
        Cursor.lockState = CursorLockMode.Confined; // Confine cursor

        playerScript.enabled = false; // Disable player controls
    }

    public void stateUnpause() // Unpause game logic
    {
        isPaused = false; // Clear pause state
        Time.timeScale = timeScaleOriginal; // Restore time scale
        Cursor.visible = false; // Hide cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor

        playerScript.enabled = true; // Enable player controls

        if (menuActive != null) // Check active menu
        {
            menuActive.SetActive(false); // Hide menu
            menuActive = null; // Clear active menu
        }
    }

    public void youLose() // Handle lose state
    {
        statePause(); // Pause game
        menuActive = menuLose; // Set lose menu
        menuActive.SetActive(true); // Show menu
    }

     public void UpdateComputerUI()
    {
        if (allNodesCollected)
        {
            if (interactWithComputerYesText != null)
            {
                interactWithComputerYesText.gameObject.SetActive(true); // Show "Yes" UI
            }
            if (interactWithComputerNoText != null)
            {
                interactWithComputerNoText.gameObject.SetActive(false); // Hide "No" UI
            }
        }
        else
        {
            if (interactWithComputerYesText != null)
            {
                interactWithComputerYesText.gameObject.SetActive(false); // Hide "Yes" UI
            }
            if (interactWithComputerNoText != null)
            {
                interactWithComputerNoText.gameObject.SetActive(true); // Show "No" UI
            }
        }
    }

    public void HideComputerUI()
    {
        if (interactWithComputerYesText != null)
        {
            interactWithComputerYesText.gameObject.SetActive(false); // Hide "Yes" UI
        }
        if (interactWithComputerNoText != null)
        {
            interactWithComputerNoText.gameObject.SetActive(false); // Hide "No" UI
        }
    }


    // Added Methods for Node UI
    public void ShowNodeInteractionUI()
    {
        if (nodeInteractionText != null)
        {
            nodeInteractionText.gameObject.SetActive(true); // Show node interaction UI
        }
    }

    public void HideNodeInteractionUI()
    {
        if (nodeInteractionText != null)
        {
            nodeInteractionText.gameObject.SetActive(false); // Hide node interaction UI
        }
    }

    // public void OnPlayerLanded() // Called when the player lands
    // {
    //     if (startScreen != null) // Check if start screen exists
    //     {
    //         startScreen.SetActive(true); // Show the start screen
    //         menuActive = startScreen;   // Set menuActive to track the start screen
    //         statePause();               // Pause the game
    //     }

    //     // Trigger the player taking damage
    //     if (playerScript != null) // Ensure the player script reference is valid
    //     {
    //         (NEEDS TO BE REMOVED AND INSURE THE PLAYER IS AT THE CORRECT HEIGHT AT START) int damageToTake = Mathf.CeilToInt(playerScript.HP * 0.9f); // Calculate 90% of the player current HP, rounded up

    //         Debug.Log($"Player Current HP: {playerScript.HP}, Damage to Take: {damageToTake}"); // Debug for clarity

    //         playerScript.takeDamage(damageToTake);   // Apply the damage
    //     }
    //     else
    //     {
    //         Debug.LogError("Player script reference is missing in GameManager!");
    //     }
    // }

    public void UpdatePlayerHealth(int currentHP, int maxHP)
    {
        if (playerHPBar != null)
        {
            // Calculate health percentage
            float healthPercentage = (float)currentHP / maxHP;

            // Update the health bar fill amount
            playerHPBar.fillAmount = healthPercentage;

            // Change the color based on health percentage
            if (healthPercentage > 0.75f)
            {
                playerHPBar.color = Color.green; // Healthy
            }
            else if (healthPercentage > 0.5f)
            {
                playerHPBar.color = Color.yellow; // Moderate
            }
            else if (healthPercentage > 0.25f)
            {
                playerHPBar.color = new Color(1f, 0.5f, 0f); // Orange
            }
            else
            {
                playerHPBar.color = Color.red; // Critical
                StartCoroutine(FlashRedHealthBar()); // Start flashing
            }
        }
    }

    // Flashing effect for critical health
    private IEnumerator FlashRedHealthBar()
    {
        while (playerHPBar.fillAmount <= 0.25f)
        {
            playerHPBar.color = new Color(1f, 0f, 0f, 0.5f); // Red with transparency
            yield return new WaitForSeconds(0.3f);
            playerHPBar.color = Color.red; // Solid red
            yield return new WaitForSeconds(0.3f);
        }
    }

}
