using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints; // array to store patrol waypoints
    private int currentWaypointIndex = 0; // track current waypoint
    private NavMeshAgent navAgent; // reference to navmesh agent

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if(waypoints.Length > 0)
        {
            navAgent.SetDestination(waypoints[currentWaypointIndex].position); // set initial destination
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length == 0) return; // if waypoints are empty, avoid dividing by zero

        if(navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // move to next waypoint
            navAgent.SetDestination(waypoints[currentWaypointIndex].position); // set new destination
        }
    }
}
