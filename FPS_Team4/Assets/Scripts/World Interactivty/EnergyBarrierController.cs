// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrierController : MonoBehaviour
{
    [Header("Barrier Settings")]
    [SerializeField] private MeshRenderer barrierRenderer; // Renderer for visual feedback
    [SerializeField] private Collider barrierCollider; // Collider to block movement
    [SerializeField] private Collider enemyBarrierCollider; // Collider to detect enemies

    [Header("Visual Settings")]
    [SerializeField] private Material activeMaterial; // Material when active
    [SerializeField] private Material inactiveMaterial; // Material when inactive
    [SerializeField] private Material deactivatingMaterial; // Material when deactivating

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Audio source for barrier sounds
    [SerializeField] private AudioClip barrierActiveLoop; // Looping sound while barrier is active
    [SerializeField] private AudioClip barrierDeactivateSound1; // First deactivation sound
    [SerializeField] private AudioClip barrierDeactivateSound2; // Second deactivation sound

    private bool isPermanentlyDisabled = false; // Tracks if the barrier is permanently disabled

    public enum BarrierState { Active, Deactivating, Inactive } // States for barrier
    private BarrierState currentState = BarrierState.Active; // Default to active

    private void Start()
    {
        SetBarrierState(BarrierState.Active); // Initialize barrier as active
    }

    public void SetBarrierState(BarrierState newState)
    {
        if (isPermanentlyDisabled) return; // Do nothing if permanently disabled

        currentState = newState; // Update current state

        switch (currentState)
        {
            case BarrierState.Active:
                barrierCollider.enabled = true; // Block movement
                barrierRenderer.material = activeMaterial; // Apply active material
                PlayActiveBarrierSound(); // Play looping sound
                break;

            case BarrierState.Deactivating:
                barrierCollider.enabled = true; // Block movement during deactivation
                barrierRenderer.material = deactivatingMaterial; // Apply deactivating material
                StopLoopingSound(); // Stop the looping sound
                PlayDeactivationSounds(); // Play deactivation sounds
                Invoke(nameof(DeactivateBarrier), 2f); // Switch to inactive after 2 seconds
                break;

            case BarrierState.Inactive:
                barrierCollider.enabled = false; // Allow movement
                barrierRenderer.material = inactiveMaterial; // Apply inactive material
                StopLoopingSound(); // Ensure all sounds are stopped
                break;
        }
    }

    private void DeactivateBarrier()
    {
        SetBarrierState(BarrierState.Inactive); // Transition to inactive state
    }

    public void PermanentlyDisableBarrier() // Permanently disable barrier when hacked
    {
        isPermanentlyDisabled = true; // Mark as permanently disabled
        barrierCollider.enabled = false; // Disable the collider
        barrierRenderer.material = inactiveMaterial; // Apply inactive material
        StopLoopingSound(); // Stop any active sounds
    }

    private void PlayActiveBarrierSound()
    {
        if (audioSource != null && barrierActiveLoop != null)
        {
            audioSource.loop = true; // Set the audio source to loop
            audioSource.clip = barrierActiveLoop; // Assign the looping clip
            audioSource.Play(); // Play the sound
        }
    }

    private void PlayDeactivationSounds()
    {
        StartCoroutine(PlayDeactivationSequence());
    }

    private IEnumerator PlayDeactivationSequence()
    {
        if (audioSource != null)
        {
            audioSource.loop = false; // Disable looping for single sounds

            if (barrierDeactivateSound1 != null)
            {
                audioSource.clip = barrierDeactivateSound1; // Assign first deactivation sound
                audioSource.Play(); // Play the sound
                yield return new WaitForSeconds(audioSource.clip.length); // Wait for it to finish
            }

            if (barrierDeactivateSound2 != null)
            {
                audioSource.clip = barrierDeactivateSound2; // Assign second deactivation sound
                audioSource.Play(); // Play the sound
                yield return new WaitForSeconds(audioSource.clip.length); // Wait for it to finish
            }
        }
    }

    private void StopLoopingSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the current sound
        }
    }

    // TESTING CODE: Press keys to test barrier states
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press 1 for Active
        {
            SetBarrierState(BarrierState.Active); // Activate barrier
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press 2 for Deactivating
        {
            SetBarrierState(BarrierState.Deactivating); // Deactivate barrier temporarily
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // Press 3 for Inactive
        {
            SetBarrierState(BarrierState.Inactive); // Fully deactivate barrier
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // Press 4 to Permanently Disable
        {
            PermanentlyDisableBarrier(); // Permanently disable barrier for testing
        }
    }
}