using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Level2Enemy2 : MonoBehaviour
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
    public Transform bulletSpawnPoint2;
    public int maxHealth = 10;
    public int currentHealth;
    public int points = 10;
    public Animator EnemyAnimator;
    public RuntimeAnimatorController fallbackAnimatorController;

    [Header("Freeze Bullet")]
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float freezeSpeedMultiplier = 0.45f;
    [SerializeField] private float freezeDuration = 1.75f;

    [SerializeField] private float spinSpeed = 180.0f;
    [SerializeField] private float flashingDuration = 1.5f;
    [SerializeField] private float deathCleanupDelay = 1.25f;
    [SerializeField] private float deathExplosionScaleMultiplier = 3.33f;
    [SerializeField] private AudioSource ShootSoundEffect;
    [SerializeField] private AudioSource DeathSoundEffect;

    private float currentSpeed;
    private float catchUpDistance = 30.0f;
    private float catchUpSpeedMultiplier = 2.0f;
    private bool isFiring;
    private bool isFlashing;
    private bool isDead;
    private float flashEndTime;
    private bool scoreAwarded;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySprite;
    private Vector3 initialLocalScale;

    private void Start()
    {
        initialLocalScale = transform.localScale;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        EnemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        enemySprite = GetComponent<SpriteRenderer>();

        if (EnemyAnimator != null && EnemyAnimator.runtimeAnimatorController == null && fallbackAnimatorController != null)
        {
            EnemyAnimator.runtimeAnimatorController = fallbackAnimatorController;
            EnemyAnimator.Rebind();
            EnemyAnimator.Update(0f);
        }

        if (enemyRigidbody != null)
        {
            enemyRigidbody.gravityScale = 0f;
            enemyRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.angularVelocity = 0f;
        }

        StartCoroutine(FireRoutine());
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        UpdateFlashState();
        Spin();
        MoveTowardsPlayer();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isFlashing)
        {
            return;
        }

        currentHealth -= damage;
        isFlashing = true;
        flashEndTime = Time.time + flashingDuration;
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fireRate);

            if (isDead || isFiring || currentHealth <= 0)
            {
                continue;
            }

            isFiring = true;
            SpawnBullet(bulletSpawnPoint != null ? bulletSpawnPoint : transform);
            SpawnBullet(bulletSpawnPoint2 != null ? bulletSpawnPoint2 : transform);
            isFiring = false;
        }
    }

    private void SpawnBullet(Transform spawnPoint)
    {
        if (enemyBulletObject == null || spawnPoint == null)
        {
            return;
        }

        GameObject enemyBullet = Instantiate(enemyBulletObject, spawnPoint.position, spawnPoint.rotation);
        enemyBullet.tag = "EnemyBulletClone";

        if (ShootSoundEffect != null)
        {
            SfxLimiter.TryPlay(ShootSoundEffect, "enemy_shoot", 0.05f, 3, 0.2f);
        }

        FreezingEnemyBullet freezingBullet = enemyBullet.GetComponent<FreezingEnemyBullet>();
        if (freezingBullet == null)
        {
            freezingBullet = enemyBullet.AddComponent<FreezingEnemyBullet>();
        }

        freezingBullet.Configure(bulletDamage, freezeSpeedMultiplier, freezeDuration);

        Rigidbody2D bulletRigidbody = enemyBullet.GetComponent<Rigidbody2D>();
        if (bulletRigidbody == null)
        {
            bulletRigidbody = enemyBullet.AddComponent<Rigidbody2D>();
        }

        bulletRigidbody.gravityScale = 0f;
        bulletRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        bulletRigidbody.angularVelocity = 0f;

        Vector2 direction = transform.TransformDirection(spawnPoint.localPosition.normalized);
        if (direction.sqrMagnitude <= 0.0001f)
        {
            direction = spawnPoint.right;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        bulletRigidbody.velocity = direction * bulletSpeed;
    }

    private void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistance)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
        else if (distanceToPlayer <= resumeDistance)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxMovementSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = maxMovementSpeed;
        }

        if (distanceToPlayer > catchUpDistance)
        {
            float remainingDistance = distanceToPlayer - stopDistance;
            float requiredSpeed = remainingDistance / Mathf.Max(stopDistance, 0.01f);
            currentSpeed = Mathf.MoveTowards(currentSpeed, requiredSpeed * catchUpSpeedMultiplier, acceleration * Time.deltaTime);
        }

        Vector3 movementDirection = (player.position - transform.position).normalized;
        transform.position += movementDirection * currentSpeed * Time.deltaTime;
    }

    private void Spin()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    private void UpdateFlashState()
    {
        if (enemySprite == null)
        {
            return;
        }

        if (!isFlashing)
        {
            enemySprite.color = Color.white;
            return;
        }

        if (Time.time >= flashEndTime)
        {
            isFlashing = false;
            enemySprite.color = Color.white;
            return;
        }

        float alpha = Mathf.PingPong(Time.time * 5.0f, 1.0f);
        enemySprite.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (DeathSoundEffect != null)
        {
            SfxLimiter.TryPlay(DeathSoundEffect, "enemy_death", 0.08f, 2, 0.25f);
        }

        currentSpeed = 0f;

        if (enemyRigidbody != null)
        {
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.angularVelocity = 0f;
            enemyRigidbody.isKinematic = true;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        transform.localScale = initialLocalScale * deathExplosionScaleMultiplier;

        if (EnemyAnimator != null && EnemyAnimator.runtimeAnimatorController != null)
        {
            EnemyAnimator.SetTrigger("Deatha");
        }

        GetComponent<DropList>()?.InstantiateLoot(transform.position);
        AwardScoreOnce();
        Destroy(gameObject, deathCleanupDelay);
    }

    private void AwardScoreOnce()
    {
        if (scoreAwarded)
        {
            return;
        }

        scoreAwarded = true;
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("PlayerBulletClone"))
        {
            return;
        }

        Destroy(collision.gameObject);
        TakeDamage(1);
    }
}
