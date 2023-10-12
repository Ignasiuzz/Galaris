using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletSpeed = 10f; // Speed of the tracking bullet
    public float trackingTime = 2f; // Time in seconds to track the target
    public Collider2D targetCollider; // The target collider to track
    public int damage = 1; // Damage inflicted by the bullet

    private float trackingTimer = 0f;
    private bool isTracking = true;
    private Vector3 lastTrackingDirection; // Store the last tracking direction

    private void Update()
    {
        if (isTracking)
        {
 

            // Calculate the center position of the target collider
            Vector3 targetPosition = targetCollider.bounds.center;

            // Calculate the direction towards the target position
            Vector3 direction = (targetPosition - transform.position).normalized;
            lastTrackingDirection = direction; // Store the last tracking direction

            // Face the bullet towards the target position
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    }
