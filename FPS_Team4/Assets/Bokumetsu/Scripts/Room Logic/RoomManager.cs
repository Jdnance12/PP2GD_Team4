using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> nextRooms;
    [SerializeField] private GameObject healthStationRoom;
    [SerializeField] private int lowHealthThreshold = 30;

    public int playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        // Deactivate all of the "next" rooms initially
        foreach (GameObject room in nextRooms) room.SetActive(false);
        if (healthStationRoom != null)
        {
            healthStationRoom.SetActive(false);
        }
    }

    public void ActivateNextRoom()
    {
        // Randomly select the next room version
        int randomRoom = Random.Range(0, nextRooms.Count);
        nextRooms[randomRoom].SetActive(true);

        // Deactivate the other rooms in the same set
        for (int i = 0; i < nextRooms.Count; i++)
        {
            if(i != randomRoom)
            {
                nextRooms[i].SetActive(false);
            }
        }

        // Check players health after defeating enemies to see if health station is needed
        CheckPlayersHealth();
    }

    void CheckPlayersHealth()
    {
        // If the player health is low after combat, activate the health station room
        if (playerHealth <= lowHealthThreshold && healthStationRoom != null)
        {
            healthStationRoom.SetActive(true);
        }
    }
}
