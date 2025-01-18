//MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Supports TextMeshPro for 3D text

public class FlashingCheckpointText : MonoBehaviour
{
    private TMP_Text text; // Reference to the TextMeshPro component
    private float flashDuration = 0.5f; // Time between flashes
    private bool isVisible = true; // Tracks if text is currently visible

    void Start()
    {
        text = GetComponent<TMP_Text>(); // Get TextMeshPro component on object
        InvokeRepeating(nameof(FlashText), flashDuration, flashDuration); // Calls FlashText repeatedly
    }

    private void FlashText()
    {
        isVisible = !isVisible; // Toggles visibility state
        text.enabled = isVisible; // Updates text visibility
    }
}