using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    GameManager gm;
    GameObject player;

    GameObject objectToFind;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        player = gm.player;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.transform.position + Vector3.up * 2.0f - transform.position;
        Quaternion rotation = Quaternion.LookRotation(-direction);
        transform.rotation = rotation;
    }
}
