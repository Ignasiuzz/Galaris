using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
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
    public Transform bulletSpawnPoint2; // Additional bullet spawn point
    public int maxHealth = 10;
    public int currentHealth;
    public int points = 10;
    public Animator EnemyAnimator;

    private float currentSpeed = 0.0f;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySprite;

    //sound
    [SerializeField] private AudioSource ShootSoundEffect;
    [SerializeField] private AudioSource DeathSoundEffect;

    private float catchUpDistance = 30.0f; // Distance to trigger catch-up behavior
    private float catchUpSpeedMultiplier = 2.0f; // Speed multiplier for catch-up behavior

    private bool isFiring = false;
    private bool isFlashing = false;
    private bool isDead = false;

    private float flashingDuration = 1.5f;
    private float flashingTimer = 0.0f;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        EnemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        enemySprite = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fireRate);

            if (!isFiring && currentHealth > 0) // Check if health is greater than 0 before firing
            {
                isFiring = true;

                // Spawn the first bullet
                SpawnBullet(bulletSpawnPoint);

                // Spawn the second bullet
                SpawnBullet(bulletSpawnPoint2);

                // Reset firing flag
                isFiring = false;
            }
        }
    }


    private void SpawnBullet(Transform spawnPoint)
    {
        GameObject enemyBullet = Instantiate(enemyBulletObject, spawnPoint.position, spawnPoint.rotation);
        enemyBullet.tag = "EnemyBulletClone";

        ShootSoundEffect.Play();
        Rigidbody2D rb = enemyBullet.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - spawnPoint.position).normalized;
        rb.velocity = direction * bulletSpeed;
    }

    private void FlashingLogic()
    {
        // Flashing logic (make the sprite transparent)
        float alpha = Mathf.PingPong(Time.time * 5.0f, 1.0f); // Adjust the multiplier for the blinking speed
        enemySprite.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    public void TakeDamage(int damage)
    {
        if (!isFlashing)
        {
            currentHealth -= damage;

            // Start flashing only when taking damage
            flashingTimer = 0.0f;
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (isFlashing)
        {
            yield break; // If already flashing, exit the coroutine
        }

        isFlashing = true;
        float flashEndTime = Time.time + flashingDuration;

        while (Time.time < flashEndTime)
        {
            FlashingLogic();
            yield return null;

            // Continue with movement logic during flashing
            if (currentHealth > 0)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Only rotate and move if the enemy is alive
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

                // Check if the distance to the player is greater than the catch-up distance
                if (distanceToPlayer > catchUpDistance)
                {
                    // Calculate the speed needed to cover the remaining distance to the player
                    float remainingDistance = distanceToPlayer - stopDistance;
                    float requiredSpeed = remainingDistance / stopDistance;

                    // Temporarily increase the speed to catch up
                    currentSpeed = Mathf.MoveTowards(currentSpeed, requiredSpeed * catchUpSpeedMultiplier, acceleration * Time.deltaTime);
                }

                Vector3 movementDirection = (player.position - transform.position).normalized;
                transform.position += movementDirection * currentSpeed * Time.deltaTime;
            }
        }

        // Reset sprite transparency
        enemySprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        isFlashing = false;
    }

    void Die()
    {
        if (!isDead) isDead = true;
        DeathSoundEffect.Play();
        currentSpeed = 0;
        EnemyAnimator.SetTrigger("Deatha");
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.angularVelocity = 0f;
        Debug.Log("Enemy died!");
        GetComponent<DropList>().InstantiateLoot(transform.position);
    }

    public void OnDeathAnimationEnd()
    {
        Debug.Log("Death animation ended");
        Destroy(gameObject);
    }

    public void Update()
    {
        if (isDead)
        {
            return; // Don't perform any actions if the enemy is dead.
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        if (player == null || isFlashing)
        {
            return; // If the player is not found or the enemy is flashing, do nothing.
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (currentHealth > 0)
        {
            // Only rotate and move if the enemy is alive
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

            // Check if the distance to the player is greater than the catch-up distance
            if (distanceToPlayer > catchUpDistance)
            {
                // Calculate the speed needed to cover the remaining distance to the player
                float remainingDistance = distanceToPlayer - stopDistance;
                float requiredSpeed = remainingDistance / stopDistance;

                // Temporarily increase the speed to catch up
                currentSpeed = Mathf.MoveTowards(currentSpeed, requiredSpeed * catchUpSpeedMultiplier, acceleration * Time.deltaTime);
            }

            Vector3 movementDirection = (player.position - transform.position).normalized;
            transform.position += movementDirection * currentSpeed * Time.deltaTime;
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
