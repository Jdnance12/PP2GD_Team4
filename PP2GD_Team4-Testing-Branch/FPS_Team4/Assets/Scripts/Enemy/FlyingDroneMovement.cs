using UnityEngine;

public class FlyingDroneMovement : MonoBehaviour
{
    [Header("Flying Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float hoverHeight = 10f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointThreshold = 1f;

    private int currentWaypointIndex = 0;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        FlyTowardsWaypoint();
        MaintainHoverHeight();
    }

    private void FlyTowardsWaypoint()
    {
        if (waypoints.Length == 0) return;

        // Get the current waypoint position
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // Move towards the waypoint
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed;

        // Rotate to face the waypoint
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the drone is close to the waypoint
        if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop to the next waypoint
        }
    }

    private void MaintainHoverHeight()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, hoverHeight, transform.position.z);
        Vector3 hoverDirection = (targetPosition - transform.position).normalized;

        // Adjust vertical position smoothly
        rb.AddForce(hoverDirection * speed, ForceMode.Acceleration);
    }
}
