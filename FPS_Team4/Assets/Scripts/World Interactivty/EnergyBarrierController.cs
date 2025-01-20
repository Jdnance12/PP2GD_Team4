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
                break;

            case BarrierState.Deactivating:
                barrierCollider.enabled = true; // Block movement during deactivation
                barrierRenderer.material = deactivatingMaterial; // Apply deactivating material
                Invoke(nameof(DeactivateBarrier), 2f); // Switch to inactive after 2 seconds
                break;

            case BarrierState.Inactive:
                barrierCollider.enabled = false; // Allow movement
                barrierRenderer.material = inactiveMaterial; // Apply inactive material
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

    private void OnTriggerEnter(Collider other)
    {
        if (isPermanentlyDisabled) return; // Do nothing if permanently disabled

        // Commented out: Ignore enemy interactions for now
        // if (other == enemyBarrierCollider) return; // Ignore trigger from enemy detection collider

        // Commented out: Handle enemy entry logic
        // if (other.CompareTag("Enemy")) // Enemy enters detection area
        // {
        //     SetBarrierState(BarrierState.Deactivating); // Deactivate barrier for enemy
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPermanentlyDisabled) return; // Do nothing if permanently disabled

        // Commented out: Handle enemy exit logic
        // if (other.CompareTag("Enemy")) // Enemy exits detection area
        // {
        //     SetBarrierState(BarrierState.Active); // Reactivate barrier for enemy
        // }
    }
}