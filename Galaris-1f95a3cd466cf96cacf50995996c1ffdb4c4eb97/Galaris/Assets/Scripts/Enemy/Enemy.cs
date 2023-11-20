using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public GameObject enemyBulletObject; // Reference to the enemy's bullet object
    public float fireRate = 1.0f; // Adjust this value for the enemy's fire rate
    public float maxMovementSpeed = 3.0f; // Maximum movement speed of the enemy
    public float acceleration = 1.0f; // Acceleration rate
    public float stopDistance = 2.0f; // Distance at which the enemy should stop
    public float resumeDistance = 5.0f; // Distance to resume acceleration
    [SerializeField] private float deceleration = 2.0f; // Deceleration rate
    public float bulletSpeed = 10.0f; // Speed of enemy bullets
    public Transform bulletSpawnPoint; // Reference to the bullet spawn point
    public int maxHealth = 10;
    public int currentHealth;
    public int points = 10; // Assign the appropriate point value for this enemy type.
    public Animator EnemyAnimator;

    private float nextFireTime;
    private float currentSpeed = 0.0f;
    private bool isDead = false;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;



    private void Start()
    {
        currentHealth = maxHealth;
        // Find the player GameObject and assign its transform to the 'player' variable
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
    void Die()
    {
        if (!isDead) isDead = true;
        enemyRigidbody.isKinematic = true;
        enemyCollider.enabled = false;
        currentSpeed = 0;
        Debug.Log("Enemy died!");
        EnemyAnimator.SetTrigger("Death");
        //Destroy(gameObject);
    }
    public void OnDeathAnimationEnd()
    {
        Debug.Log("Death animation ended");
        Destroy(gameObject);
    }
    private void Update()
    {
        if (isDead)
        {
            return; // Don't perform any actions if the enemy is dead.
        }

        if (player == null)
            return; // If the player is not found, do nothing.

        // Rotate the enemy to face the player
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update speed based on the distance
        if (distanceToPlayer <= stopDistance)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
        else if (distanceToPlayer > stopDistance && distanceToPlayer <= resumeDistance)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxMovementSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = maxMovementSpeed;
        }
        if (currentHealth <= 0)
        {
            Die();
        }

        // Move the enemy towards the player
        Vector3 movementDirection = (player.position - transform.position).normalized;
        transform.position += movementDirection * currentSpeed * Time.deltaTime;

        // Check if it's time to fire
        if (Time.time > nextFireTime)
        {
            Fire(); // Fire an enemy bullet
            nextFireTime = Time.time + 1 / fireRate; // Set the next fire time based on fire rate
        }
    }

    private void Fire()
    {
        // Check if the enemyBulletObject is assigned in the Inspector
        if (enemyBulletObject != null)
        {
            // Create an instance of the enemy bullet object
            GameObject enemyBullet = Instantiate(enemyBulletObject, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            enemyBullet.tag = "EnemyBulletClone";

            // Set the speed and direction of the bullet based on the player's position
            Rigidbody2D rb = enemyBullet.GetComponent<Rigidbody2D>();
            Vector2 direction = (player.position - bulletSpawnPoint.position).normalized;
            rb.velocity = direction * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Enemy bullet object is not assigned in the Inspector!");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBulletClone"))
        {
            Debug.Log("Player bullet collided with the enemy");

            // Destroy the enemyBullet
            Destroy(collision.gameObject);
            TakeDamage(1);

        }
    }
    void OnDestroy()
    {
        FindObjectOfType<ScoreManager>().AddScore(points);
    }
}