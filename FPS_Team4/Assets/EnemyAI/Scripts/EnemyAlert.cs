using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlert : MonoBehaviour
{

    public List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    public void AlertEnemies(Vector3 position)
    {
        foreach(GameObject enemy in enemiesInRange)
        {
            enemy.GetComponent<EnemyAI>().MoveToPosition(position);
        }
    }
}
