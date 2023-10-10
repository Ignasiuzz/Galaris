using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class shooting : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootingInterval = 1f; // Interval between shots.
    public float bulletSpeed = 10f; // Adjust as needed.
    public LayerMask collisionLayers; // Set in the Inspector to specify which layers should trigger deletion;
    public GameObject selectedEnemy; // Reference to the manually selected enemy.

    private PlayableArea playableArea; // Reference to the PlayableArea script.
    private bool isShooting = false; // Flag to control shooting;

    private void Start()
    {
        // Find the PlayableArea GameObject in the scene.
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
    }

    private void Update()
    {
        // Check if the fire button is being held down.
        if (isShooting && selectedEnemy != null)
        {
            // Calculate the direction to the selected enemy.
            Vector2 direction = selectedEnemy.transform.position - transform.position;

            // Update the player's rotation to continuously face the selected enemy.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // When the button is pressed, start shooting.
        StartShooting();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // When the button is released, stop shooting.
        StopShooting();
    }

    public void StartShooting()
    {
        isShooting = true;
        // Start shooting every shootingInterval seconds.
        InvokeRepeating("Shoot", 0f, shootingInterval);
    }

    public void StopShooting()
    {
        isShooting = false;
        // Stop the shooting interval when the button is released.
        CancelInvoke("Shoot");
    }

    public void Shoot()
    {
        // Calculate the direction to shoot the bullet.
        Vector3 shootingDirection = selectedEnemy.transform.position - transform.position;
        shootingDirection.Normalize();

        // Instantiate the bullet.
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, shootingDirection));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Set the bullet's speed independently of force.
        rb.velocity = bulletSpeed * shootingDirection;

        // Attach a script to the bullet to handle collision detection and deletion.
        BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();

        // Call the Initialize method to set collisionLayers and playableArea.
        collisionHandler.Initialize(collisionLayers, playableArea);
    }
}
