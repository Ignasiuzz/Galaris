using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    public float speed = 10f;
    public float trackingDistance = 5f; // Distance to track the player before stopping
    private Transform player;
    private Vector3 initialDirection;
    private bool isTracking = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialDirection = (player.position - transform.position).normalized;
    }

    void Update()
    {
        if (isTracking)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 trackingForce = directionToPlayer * speed;
            transform.Translate(trackingForce * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer >= trackingDistance)
            {
                isTracking = false;
            }
        }
        else
        {
            // Continue moving in the initial direction
            transform.Translate(initialDirection * speed * Time.deltaTime);
        }
    }
}
