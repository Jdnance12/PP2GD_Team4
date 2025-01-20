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

    private bool isPlayerInRange = false; // Tracks if player is near terminal
    private bool isHacked = false; // Tracks if terminal is already hacked
    private bool isHackingFailed = false; // Tracks if hacking failed
    private bool isHackingActive = false; // Tracks if the hacking sequence is active
    private bool hasGeneratedLetters = false; // Tracks if letters were successfully generated
    private List<char> alphabet = new List<char>() { 'b', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 'u', 'v', 'x', 'y', 'z' }; // Simplified alphabet list
    private List<char> targetLetters = new List<char>(); // Stores target letters
    private Coroutine hackingCoroutine; // Tracks ongoing hacking coroutine
    private int currentChallengeIndex = 0; // Tracks the current challenge index

    private void Start()
    {
        if (hackingCanvas != null)
            hackingCanvas.SetActive(true); // Ensure hacking Canvas is always visible

        if (hackingPrompt != null)
            hackingPrompt.text = ""; // Clear the text at the start

        if (turretPrefab != null)
            turretPrefab.SetActive(false); // Ensure turret starts deactivated
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.T) && !isHacked && !isHackingFailed)
        {
            if (!isHackingActive) // Ensure hacking only starts once per session
            {
                StartCoroutine(PreparationDelay()); // Add delay for smoother transition
            }
        }

        if (isHackingActive && targetLetters.Count > 0) // Check for active hacking session
        {
            CheckPlayerInput(); // Check player input for correct or wrong keys
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detect if player enters terminal range
        {
            isPlayerInRange = true; // Mark player as in range
            if (!isHacked && !isHackingFailed)
            {
                hackingPrompt.text = "Press T to interact with terminal."; // Initial interaction prompt
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detect if player leaves terminal range
        {
            isPlayerInRange = false; // Mark player as out of range
            hackingPrompt.text = ""; // Reset interaction prompt
        }
    }

    private IEnumerator PreparationDelay()
    {
        if (isHackingActive) yield break; // Prevent multiple triggers
        ResetHackingState(); // Ensure the state is reset before starting
        isHackingActive = true; // Mark hacking as active
        hackingPrompt.text = "Prepare for hacking..."; // Notify player of preparation
        yield return new WaitForSeconds(1.5f); // Delay before starting instructions

        GenerateRandomLetters(); // Generate the three random letters
        Debug.Log($"Generated letters valid: {hasGeneratedLetters}, Count: {targetLetters.Count}"); // Debugging log
        if (!hasGeneratedLetters || targetLetters.Count == 0)
        {
            Debug.LogError("Failed to generate target letters. Aborting hacking sequence.");
            isHackingActive = false; // Reset state on failure
            yield break; // Stop here if no letters were generated
        }

        hackingPrompt.text = "To bypass the barrier, press the correct letters as they appear. Press T when ready."; // Update prompt
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.T)); // Wait for the second T press

        hackingCoroutine = StartCoroutine(HackingSequence()); // Start the hacking sequence
    }

    private IEnumerator HackingSequence()
    {
        float totalTimer = 4f; // Total time allowed for hacking
        currentChallengeIndex = 0; // Reset challenge index

        if (targetLetters.Count == 0) // Ensure target letters exist
        {
            Debug.LogError("No target letters generated. Exiting sequence.");
            yield break;
        }

        hackingPrompt.text = $"Press {targetLetters[currentChallengeIndex]}"; // Show the first challenge letter

        while (totalTimer > 0)
        {
            if (isHacked) yield break; // Stop timer if hacking succeeds

            totalTimer -= Time.deltaTime;

            if (currentChallengeIndex >= targetLetters.Count) // All challenges completed
            {
                CompleteHacking(); // Trigger success
                yield break; // Exit the coroutine
            }

            yield return null; // Wait for next frame
        }

        if (!isHacked) ActivateTurret(); // Only activate turret if hacking failed
    }

    private void GenerateRandomLetters()
    {
        targetLetters.Clear(); // Clear any existing letters
        hasGeneratedLetters = false; // Reset generated flag

        HashSet<char> uniqueLetters = new HashSet<char>(); // Ensure unique letters are selected

        while (uniqueLetters.Count < 3) // Ensure three unique letters are generated
        {
            char randomLetter = alphabet[Random.Range(0, alphabet.Count)]; // Pick a random letter from the alphabet
            uniqueLetters.Add(randomLetter);
        }

        targetLetters.AddRange(uniqueLetters);

        if (targetLetters.Count == 3)
        {
            hasGeneratedLetters = true; // Mark as successfully generated
            Debug.Log($"Generated letters: {string.Join(", ", targetLetters)}"); // Log generated letters
        }
        else
        {
            Debug.LogError("Letter generation failed unexpectedly."); // Fallback error log
        }
    }

    private void CheckPlayerInput()
    {
        if (currentChallengeIndex >= targetLetters.Count) return; // Prevent out-of-bounds access

        if (Input.anyKeyDown) // Ensure only one key press is handled per frame
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    char pressedChar = key.ToString().ToLower()[0]; // Convert KeyCode to lowercase char
                    char expectedChar = targetLetters[currentChallengeIndex]; // Get expected char

                    Debug.Log($"Pressed: {pressedChar}, Expected: {expectedChar}"); // Debug input and expected value

                    if (pressedChar == expectedChar) // Check if input matches
                    {
                        Debug.Log($"Correct key pressed: {pressedChar}");
                        currentChallengeIndex++; // Move to next challenge

                        if (currentChallengeIndex < targetLetters.Count)
                        {
                            hackingPrompt.text = $"Press {targetLetters[currentChallengeIndex]}"; // Update prompt
                        }
                        else
                        {
                            CompleteHacking(); // All letters completed successfully
                            StopHackingCoroutine(); // Stop coroutine
                        }
                    }
                    else
                    {
                        Debug.Log($"Wrong key pressed! Expected: {expectedChar}, but got: {pressedChar}");
                        ActivateTurret(); // Trigger failure
                        StopHackingCoroutine(); // Stop coroutine
                    }

                    break; // Stop after processing one keypress
                }
            }
        }
    }

    private void CompleteHacking()
    {
        if (isHackingFailed) return; // Prevent success logic if already failed

        hackingPrompt.text = "Hacking successful! Barrier deactivated permanently!"; // Show success message
        linkedBarrier?.PermanentlyDisableBarrier(); // Disable the linked barrier
        isHacked = true; // Mark terminal as permanently hacked
        isHackingActive = false; // Reset hacking state
        targetLetters.Clear(); // Clear the challenges
        turretPrefab?.SetActive(false); // Ensure turret is deactivated
        StopHackingCoroutine(); // Stop active hacking
    }

    private void ActivateTurret()
    {
        if (isHacked || !isHackingFailed) return; // Prevent turret activation on success

        Debug.Log("Hacking failed! Timer expired or wrong key pressed.");
        turretPrefab?.SetActive(true); // Activate the turret
        hackingPrompt.text = "Hacking failed! Turret activated!"; // Show failure message
        isHackingFailed = true; // Mark hacking as failed
        StopHackingCoroutine(); // Stop active hacking
        StartCoroutine(ResetTerminalAfterDelay(5f)); // Reset terminal after delay
    }

    private void ResetHackingState()
    {
        targetLetters.Clear(); // Clear previous challenges
        currentChallengeIndex = 0; // Reset challenge index
        isHackingFailed = false; // Reset hacking failed state
        isHackingActive = false; // Ensure hacking is inactive initially
        hasGeneratedLetters = false; // Reset generated letters flag
    }

    private IEnumerator ResetTerminalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for reset delay
        if (!isHacked) // Only reset if terminal isnt permanently hacked
        {
            ResetHackingState(); // Reset the hacking state
            if (isPlayerInRange) // Only show prompt if player is still in range
            {
                hackingPrompt.text = "Press T to interact with terminal."; // Reset prompt
            }
            else
            {
                hackingPrompt.text = ""; // Clear the prompt if player has left the range
            }
            turretPrefab?.SetActive(false); // Deactivate turret
        }
    }

    private void StopHackingCoroutine()
    {
        if (hackingCoroutine != null) // Check if coroutine exists
        {
            StopCoroutine(hackingCoroutine); // Stop active coroutine
            hackingCoroutine = null; // Clear reference
        }
    }
}