using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;
    public Transform player;

    private float timeSinceLastShot;
    public float shootingInterval = 1f;

    public GameObject enemyBulletPrefab;
    public float bulletSpeed = 10f;
    public Transform firePoint; // The fixed point from which bullets are fired

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeSinceLastShot = 0f;
    }

    void Update()
    {
        // Calculate the direction towards the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Rotate the enemy to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && (Vector2.Distance(transform.position, player.position) > retreatDistance))
        {
            transform.position = this.transform.position;
        }

        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= shootingInterval)
        {
            Shoot();
            timeSinceLastShot = 0f;
        }
    }

    void Shoot()
    {
        // Calculate the direction towards the player
        Vector3 direction = (player.position - firePoint.position).normalized;

        // Instantiate a new bullet from the fixed fire point
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);

        // Set the bullet's velocity to track the player
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }
}