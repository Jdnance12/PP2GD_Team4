// MK FILE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Makes class serializable for JSON
public class SaveData
{
    public float[] playerPosition; // Stores player position as array
    public float currentHP; // Stores current health
    public float maxHP; // Stores max health
    public float currentShield; // Stores current shield value
    public float maxShield; // Stores max shield value
    public int nodeCount; // Tracks nodes collected
    public int partsCount; // Tracks parts collected
    public bool[] progressionFlags; // Tracks game progression states
}