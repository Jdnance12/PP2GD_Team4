// MK FILE
using System.IO; // Provides file handling methods
using UnityEngine; // Unity engine support
public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/saveData.json"; // Sets file path save data

    public static void SaveGame(GameManager gameManager) // Saves game state file
    {
        SaveData saveData = new SaveData(); // Creates new save data object

        saveData.playerPosition = new float[] // Saves player position as array
        {
            gameManager.player.transform.position.x,
            gameManager.player.transform.position.y,
            gameManager.player.transform.position.z  
        };

        saveData.currentHP = gameManager.playerScript.GetCurrentHP(); // Saves current player health
        saveData.maxHP = gameManager.playerScript.GetMaxHP(); // Saves max player health
        saveData.currentShield = gameManager.playerScript.maxShield; // Saves current shield value
        saveData.maxShield = gameManager.playerScript.maxShield; // Saves max shield value

        saveData.nodeCount = gameManager.nodeCount; // Saves node count
        saveData.partsCount = gameManager.partsCount; // Saves parts count

        saveData.progressionFlags = new bool[] // Saves progression states as bools
        {
            gameManager.progressScript.tutorialActive, // Tracks tutorial is active
            gameManager.progressScript.firstBossKilled // Tracks first boss is killed
        };

        string json = JsonUtility.ToJson(saveData, true); // Converts save data JSON format
        File.WriteAllText(SavePath, json); // Writes JSON save file
        Debug.Log("Game saved to: " + SavePath); // Logs save confirmation
    }

    public static SaveData LoadGame() // Loads game state from file
    {
        if (File.Exists(SavePath)) // Checks if save file exists
        {
            string json = File.ReadAllText(SavePath); // Reads JSON from save file
            SaveData saveData = JsonUtility.FromJson<SaveData>(json); // Converts JSON save data object
            Debug.Log("Game loaded"); // Logs load confirmation
            return saveData; // Returns save data object
        }
        else
        {
            Debug.LogWarning("Save file not found"); // Logs missing save file
            return null; // Returns null no file exists
        }
    }   
    public static bool DoesSaveFileExist() // Checks if save file exists at specified path
    {
        return File.Exists(SavePath); // Returns true if file exists
    }
}