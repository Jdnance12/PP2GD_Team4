using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    
    public Transform[] GetWaypoints()
    {
        return waypoints.ToArray(); // return the full set of waypoints
    }
}
