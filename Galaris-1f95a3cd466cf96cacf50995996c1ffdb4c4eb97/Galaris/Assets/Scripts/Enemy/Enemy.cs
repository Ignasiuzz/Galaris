using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public GameObject enemyBulletObject;
    public float fireRate = 1.0f;
    public float maxMovementSpeed = 3.0f;
    public float acceleration = 1.0f;
    public float stopDistance = 2.0f;
    public float resumeDistance = 5.0f;
    [SerializeField] private float deceleration = 2.0f;
    public float bulletSpeed = 10.0f;
    public Transform bulletSpawnPoint;
    public int maxHealth = 10;
    public int currentHealth;
    public int points = 10;
    public Animator EnemyAnimator;

    private float nextFireTime;
    private float currentSpeed = 0.0f;
    private bool isDead = false;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;

    private float catchUpDistance = 30.0f; // Distance to trigger catch-up behavior
    private float catchUpSpeedMultiplier = 2.0f; // Speed multiplier for catch-up behavior

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.angularVelocity = 0f;
        Debug.Log("Enemy died!");
        EnemyAnimator.SetTrigger("Death");
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
        {
            return; // If the player is not found, do nothing.
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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

        Vector3 movementDirection = (player.position - transform.position).normalized;
        transform.position += movementDirection * currentSpeed * Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1 / fireRate;
        }

        // Check if the distance to the player is greater than the catch-up distance
        if (distanceToPlayer > catchUpDistance)
        {
            // Calculate the speed needed to cover the remaining distance to the player
            float remainingDistance = distanceToPlayer - stopDistance;
            float requiredSpeed = remainingDistance / stopDistance;

            // Temporarily increase the speed to catch up
            currentSpeed = Mathf.MoveTowards(currentSpeed, requiredSpeed * catchUpSpeedMultiplier, acceleration * Time.deltaTime);
        }
    }

    private void Fire()
    {
        if (enemyBulletObject != null)
        {
            GameObject enemyBullet = Instantiate(enemyBulletObject, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            enemyBullet.tag = "EnemyBulletClone";

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
            Destroy(collision.gameObject);
            TakeDamage(1);
        }
    }

    void OnDestroy()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
    }
}
