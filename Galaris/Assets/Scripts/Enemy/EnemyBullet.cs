using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    public float bulletSpeed = 10f; // Speed of the tracking bullet
    public float trackingTime = 2f; // Time in seconds to track the player
    public Transform target; // The target to track

    private float trackingTimer = 0f;
    private bool isTracking = true;
    private Vector3 lastTrackingDirection; // Store the last tracking direction

    private void Update()
    {
        if (isTracking)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Calculate the direction towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            lastTrackingDirection = direction; // Store the last tracking direction

            // Face the bullet towards the player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Move the bullet
            transform.Translate(direction * bulletSpeed * Time.deltaTime, Space.World);

            trackingTimer += Time.deltaTime;

            if (trackingTimer >= trackingTime)
            {
                // Stop tracking and continue straight
                isTracking = false;
                trackingTimer = 0f;
            }
        }
        else
        {
            // Continue moving in the last tracking direction
            transform.Translate(lastTrackingDirection * bulletSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the bullet when it enters any trigger collider.
        Destroy(gameObject);
    }
}

