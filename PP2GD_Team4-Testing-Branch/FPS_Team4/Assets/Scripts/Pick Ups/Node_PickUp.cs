using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_PickUp : MonoBehaviour
{
    public GameManager gameManager;

    public float fallSpeed;
    public float raycastDist;

    private bool isFalling;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        isFalling = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * raycastDist, Color.red);

        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDist))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    isFalling = false;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int nodeAmount = 1;
            gameManager.AddNodeCount(nodeAmount);

            Destroy(gameObject);
        }
    }
}
