using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face_Player : MonoBehaviour
{
    gameManager gm;
    GameObject player;

    private void Start()
    {
        gm = gameManager.instance;

        player = gameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(-direction);
        transform.rotation = rotation;
    }
}
