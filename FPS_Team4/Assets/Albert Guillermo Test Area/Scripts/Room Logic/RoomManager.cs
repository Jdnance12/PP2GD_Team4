using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<DoorRooms> nextRooms; // List of lists to hold rooms variation for each door
    [SerializeField] private GameObject healthStationRoom;
    [SerializeField] private float lowHealthThresholdPercentage = .3f;


    // Start is called before the first frame update
    void Start()
    {
        // Deactivate all of the "next" rooms initially
        foreach(var doorRooms in nextRooms)
        {
            foreach (GameObject room in doorRooms.rooms)
            {
                if (room != null) // check if null (no next room) (boss room?)
                {
                    room.SetActive(false);
                }
            }
        }
        
        if (healthStationRoom != null)
        {
            healthStationRoom.SetActive(false);
        }
    }

    public void ActivateNextRoom(int doorIndex)
    {
        if(doorIndex < 0 || doorIndex >= nextRooms.Count)
        {
            // should cover any weird error from out of bounds entries.
            Debug.LogError("Invalid door index");
            return;
        }
        // Randomly select the next room version
        int randomRoom = Random.Range(0, nextRooms[doorIndex].rooms.Count);
        nextRooms[doorIndex].rooms[randomRoom].SetActive(true);

        // Deactivate the other rooms in the same set
        for (int i = 0; i < nextRooms[doorIndex].rooms.Count; i++)
        {
            if(i != randomRoom)
            {
                nextRooms[doorIndex].rooms[i].SetActive(false);
            }
        }

        // Check players health after defeating enemies to see if health station is needed
        CheckPlayersHealth();
    }

    void CheckPlayersHealth()
    {
        // Code assumes player game object has tag of "Player". also check to see if player exists
        PlayerController playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            float currentHP = playerController.GetCurrentHP();
            float maxHP = playerController.GetMaxHP();
            float healthPercentage = currentHP / maxHP;
            // If the player health is low after combat, activate the health station room
            if (healthPercentage <= lowHealthThresholdPercentage && healthStationRoom != null)
            {
                healthStationRoom.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("PlayerController not found on the Player Game Object");
        }
    }
}
