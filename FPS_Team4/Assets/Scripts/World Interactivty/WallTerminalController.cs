//MK File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import for dynamic text

public class WallTerminalController : MonoBehaviour
{
    [Header("Terminal Settings")]
    [SerializeField] private EnergyBarrierController linkedBarrier; // Reference to linked barrier
    [SerializeField] private GameObject hackingCanvas; // Canvas displayed during hacking
    [SerializeField] private TMP_Text hackingPrompt; // Dynamic text prompt for player
    [SerializeField] private GameObject turretPrefab; // Turret to activate on hacking failure
    [SerializeField] private GameObject enemyPrefab; // Basic enemy to spawn on hacking failure
    [SerializeField] private Transform enemySpawnPoint; // Spawn location for the enemy

    private bool isPlayerInRange = false; // Tracks if player is near terminal
    private bool isHacked = false; // Tracks if terminal is already hacked
    private bool isHackingFailed = false; // Tracks if hacking failed
    private bool isHackingActive = false; // Tracks if hacking is active
    private bool isInInstructionPhase = false; // Tracks if player is in instruction phase
    private bool isInputPhaseActive = false; // Tracks if input phase is active
    private List<char> targetLetters = new List<char>(); // Stores target letters
    private int currentChallengeIndex = 0; // Tracks current challenge index
    private float hackingTimer = 4f; // Timer for hack duration
    private bool playerExitedDuringPhase = false; // Tracks if player exited during hacking
    private bool isTerminalLocked = false; // Tracks if terminal is locked

    private void Start()
    {
        if (hackingCanvas != null)
            hackingCanvas.SetActive(true); // Show hacking canvas initially

        if (hackingPrompt != null)
            hackingPrompt.text = ""; // Clear text at start

        if (turretPrefab != null)
            turretPrefab.SetActive(false); // Ensure turret starts inactive

        if (enemyPrefab != null)
            enemyPrefab.SetActive(false); // Ensure enemy is disabled initially
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.T) && !isHacked && !isTerminalLocked)
        {
            if (!isHackingActive && !isInInstructionPhase && !isInputPhaseActive) // Ensure correct phase order
            {
                StartInstructionPhase(); // Start instruction phase
            }
            else if (isInInstructionPhase) // Move from instruction to preparation phase
            {
                StartCoroutine(StartPreparationPhase()); // Activate preparation phase
            }
        }

        if (isInputPhaseActive && targetLetters.Count > 0) // Check if input phase is active
        {
            hackingTimer -= Time.deltaTime; // Countdown timer
            if (playerExitedDuringPhase) // Check if player exited
            {
                ResetTerminal(); // Reset terminal if player exits
                return;
            }

            if (hackingTimer <= 0f) // Timer expires
            {
                TriggerHackingFailure(); // Handle hacking failure
            }
            else
            {
                CheckPlayerInput(); // Handle player's key input
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detect player entering terminal range
        {
            isPlayerInRange = true; // Mark player as in range
            playerExitedDuringPhase = false; // Reset exit tracker

            if (isTerminalLocked) // Check if terminal is locked
            {
                hackingPrompt.text = "Terminal Locked for 10 seconds!"; // Notify player of lock
                return; // Exit early
            }

            if (!isHacked && !isHackingFailed && !isHackingActive) // Ensure terminal is ready
            {
                hackingPrompt.text = "Press T to interact with terminal."; // Prompt player to interact
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detect player exiting terminal range
        {
            isPlayerInRange = false; // Mark player as out of range
            hackingPrompt.text = ""; // Clear interaction prompt
            if (isHackingActive || isInputPhaseActive || isInInstructionPhase) // If hacking is in progress
            {
                playerExitedDuringPhase = true; // Mark player exited during phase
                ResetTerminal(); // Reset the terminal immediately
            }
        }
    }

    private void StartInstructionPhase()
    {
        ResetHackingState(); // Reset previous hacking state
        isHackingActive = true; // Mark hacking as active
        isInInstructionPhase = true; // Mark instruction phase active

        hackingPrompt.text = "To bypass the barrier, press the correct letters as they appear. The letters appear in a random order. If the hack isn't complete in 4 seconds after the first letter appears countermeasures will be activated to the hack and its location.\n\nPress T to proceed."; // Show detailed instructions
    }

    private IEnumerator StartPreparationPhase()
    {
        isInInstructionPhase = false; // End instruction phase
        hackingPrompt.text = "Prepare for hacking..."; // Show preparation message
        yield return new WaitForSeconds(2f); // Wait 2 seconds for preparation

        if (playerExitedDuringPhase) // Reset terminal if player exits during preparation
        {
            ResetTerminal();
            yield break;
        }

        StartInputPhase(); // Transition to input phase
    }

    private void StartInputPhase()
    {
        isInputPhaseActive = true; // Mark input phase as active
        hackingTimer = 4f; // Reset hacking timer

        GenerateRandomLetters(); // Generate the random letters for hacking
        if (targetLetters.Count == 0) // If no letters generated, abort process
        {
            Debug.LogError("Failed to generate target letters."); // Log error
            isHackingActive = false; // Reset active state
            isInputPhaseActive = false; // Reset input phase
            return; // Stop process
        }

        hackingPrompt.text = $"Press {targetLetters[0]}"; // Display first letter
        Debug.Log($"Generated letters: {string.Join(", ", targetLetters)}"); // Log letters
    }

    private void GenerateRandomLetters()
    {
        targetLetters.Clear(); // Clear previous letters
        char[] validLetters = { 'h', 'j', 'k', 'l' }; // Set valid letters
        HashSet<char> uniqueLetters = new HashSet<char>();

        while (uniqueLetters.Count < 4) // Ensure 4 unique letters are generated
        {
            char randomLetter = validLetters[Random.Range(0, validLetters.Length)];
            uniqueLetters.Add(randomLetter);
        }

        targetLetters.AddRange(uniqueLetters); // Add generated letters
    }

    private void CheckPlayerInput()
    {
        if (currentChallengeIndex >= targetLetters.Count) return; // Prevent out-of-bounds errors

        if (Input.anyKeyDown) // Check if any key is pressed
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) // Loop through keys
            {
                if (Input.GetKeyDown(key))
                {
                    char pressedChar = key.ToString().ToLower()[0]; // Convert to lowercase
                    char expectedChar = targetLetters[currentChallengeIndex]; // Get expected letter

                    if (pressedChar == expectedChar) // Check match
                    {
                        currentChallengeIndex++; // Advance to next letter
                        hackingTimer = 4f; // Reset the timer for the next letter
                        if (currentChallengeIndex < targetLetters.Count)
                        {
                            hackingPrompt.text = $"Press {targetLetters[currentChallengeIndex]}"; // Update prompt
                        }
                        else
                        {
                            CompleteHacking(); // Hacking successful
                        }
                    }
                    else
                    {
                        TriggerHackingFailure(); // Handle failure on incorrect key
                    }

                    break; // Stop after handling one key
                }
            }
        }
    }

    private void CompleteHacking()
    {
        if (isHackingFailed) return; // Skip if already failed

        hackingPrompt.text = "Hacking successful! Barrier deactivated permanently!"; // Success message

        // Trigger the deactivation process of the linked barrier
        if (linkedBarrier != null)
        {
            linkedBarrier.SetBarrierState(EnergyBarrierController.BarrierState.Deactivating); // Start deactivation process
        }

        isHacked = true; // Mark as hacked
        isHackingActive = false; // Reset hacking state
        isInputPhaseActive = false; // End input phase
        targetLetters.Clear(); // Clear challenges
        turretPrefab?.SetActive(false); // Deactivate turret
    }

    private void TriggerHackingFailure()
    {
        isHackingFailed = true; // Mark hacking as failed
        isInputPhaseActive = false; // End input phase
        isTerminalLocked = true; // Lock the terminal

        if (enemyPrefab != null) // Enable the enemy on failure
        {
            enemyPrefab.SetActive(true); // Activate the enemy prefab
            enemyPrefab.transform.position = enemySpawnPoint.position; // Set enemy position
            enemyPrefab.transform.rotation = enemySpawnPoint.rotation; // Set enemy rotation
        }

        if (turretPrefab != null)
        {
            turretPrefab.SetActive(true); // Activate turret
        }

        StartCoroutine(DisplayFailureMessages()); // Start failure sequence
    }

    private IEnumerator DisplayFailureMessages()
    {
        hackingPrompt.color = Color.red; // Change text color to red

        hackingPrompt.text = "Hacking failed! Turret activated."; // Display failure message
        yield return new WaitForSeconds(5f); // Display for 5 seconds

        hackingPrompt.text = "Terminal Locked for 10 seconds!"; // Notify player of lock
        yield return new WaitForSeconds(10f); // Lock duration

        isTerminalLocked = false; // Unlock terminal
        hackingPrompt.color = Color.white; // Reset text color to white
        ResetTerminal(); // Reset terminal state
    }

    private void ResetTerminal()
    {
        ResetHackingState(); // Reset all hacking states
        if (isPlayerInRange && !isHacked && !isTerminalLocked) // Show interaction prompt if player is near and not hacked
        {
            hackingPrompt.text = "Press T to interact with terminal."; // Reset interaction prompt
        }
    }

    private void ResetHackingState()
    {
        targetLetters.Clear(); // Clear challenges
        currentChallengeIndex = 0; // Reset index
        isHackingFailed = false; // Reset failed state
        isHackingActive = false; // Reset hacking state
        isInInstructionPhase = false; // Reset instruction phase
        isInputPhaseActive = false; // Reset input phase
        hackingTimer = 4f; // Reset timer
        playerExitedDuringPhase = false; // Reset player exit flag
    }
}