using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Level3Enemy1 : MonoBehaviour
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

    [Header("Burn Bullet")]
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float burnTickDamage = 0.2f;
    [SerializeField] private int burnTickCount = 12;
    [SerializeField] private float burnTickInterval = 0.5f;
    [SerializeField] private float spawnProtectionDuration = 0.35f;
    [SerializeField] private float deathCleanupDelay = 1.25f;
    [SerializeField] private float deathExplosionScaleMultiplier = 1.9f;
    [SerializeField] private float shootSfxVolume = 0.2f;
    [SerializeField] private float deathSfxVolume = 0.3f;

    private float nextFireTime;
    private float currentSpeed = 0.0f;
    private bool isDead = false;
    private float spawnProtectionEndTime;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private SpriteRenderer enemySpriteRenderer;
    private Vector3 initialLocalScale;

    [SerializeField] private AudioSource ShootSoundEffect;
    [SerializeField] private AudioSource DeathSoundEffect;

    private float catchUpDistance = 30.0f;
    private float catchUpSpeedMultiplier = 2.0f;

    private void Start()
    {
        initialLocalScale = transform.localScale;
        currentHealth = maxHealth;
        spawnProtectionEndTime = Time.time + spawnProtectionDuration;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        EnemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();

        if (ShootSoundEffect != null)
        {
            ShootSoundEffect.volume = shootSfxVolume;
        }

        if (DeathSoundEffect != null)
        {
            DeathSoundEffect.volume = deathSfxVolume;
        }

        if (EnemyAnimator != null)
        {
            EnemyAnimator.Rebind();
            EnemyAnimator.Update(0f);
        }

        if (enemyRigidbody != null)
        {
            enemyRigidbody.gravityScale = 0f;
            enemyRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            enemyRigidbody.isKinematic = false;
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.angularVelocity = 0f;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }

        if (enemySpriteRenderer != null)
        {
            enemySpriteRenderer.color = Color.white;
        }

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
            SfxLimiter.TryPlay(DeathSoundEffect, "level3_enemy_death", 0.2f, 1, 0.45f);
        }

        enemyRigidbody.isKinematic = true;
        enemyCollider.enabled = false;
        currentSpeed = 0;
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.angularVelocity = 0f;
        transform.localScale = initialLocalScale * deathExplosionScaleMultiplier;
        if (EnemyAnimator != null)
        {
            EnemyAnimator.SetTrigger("Death");
        }

        GetComponent<DropList>()?.InstantiateLoot(transform.position);
        Destroy(gameObject, deathCleanupDelay);
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.up = direction;

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

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        Vector3 movementDirection = (player.position - transform.position).normalized;
        transform.position += movementDirection * currentSpeed * Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1 / fireRate;
        }

        if (distanceToPlayer > catchUpDistance)
        {
            float remainingDistance = distanceToPlayer - stopDistance;
            float requiredSpeed = remainingDistance / stopDistance;
            currentSpeed = Mathf.MoveTowards(currentSpeed, requiredSpeed * catchUpSpeedMultiplier, acceleration * Time.deltaTime);
        }
    }

    private void Fire()
    {
        if (enemyBulletObject == null)
        {
            Debug.LogWarning("Level3Enemy1 bullet prefab is not assigned.");
            return;
        }

        Transform spawnPoint = bulletSpawnPoint != null ? bulletSpawnPoint : transform;
        GameObject enemyBullet = Instantiate(enemyBulletObject, spawnPoint.position, spawnPoint.rotation);
        enemyBullet.tag = "EnemyBulletClone";

        if (ShootSoundEffect != null)
        {
            SfxLimiter.TryPlay(ShootSoundEffect, "level3_enemy_shoot", 0.12f, 1, 0.3f);
        }

        BurningEnemyBullet burningBullet = enemyBullet.GetComponent<BurningEnemyBullet>();
        if (burningBullet == null)
        {
            burningBullet = enemyBullet.AddComponent<BurningEnemyBullet>();
        }

        burningBullet.Configure(bulletDamage, burnTickDamage, burnTickCount, burnTickInterval);

        Rigidbody2D bulletRigidbody = enemyBullet.GetComponent<Rigidbody2D>();
        if (bulletRigidbody == null)
        {
            bulletRigidbody = enemyBullet.AddComponent<Rigidbody2D>();
        }

        Collider2D bulletCollider = enemyBullet.GetComponent<Collider2D>();
        if (bulletCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, enemyCollider, true);
        }

        if (bulletRigidbody != null)
        {
            bulletRigidbody.gravityScale = 0f;
            bulletRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (bulletRigidbody != null && player != null)
        {
            Vector2 direction = (player.position - spawnPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            bulletRigidbody.velocity = direction * bulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBulletClone"))
        {
            if (Time.time < spawnProtectionEndTime)
            {
                Destroy(collision.gameObject);
                return;
            }

            Destroy(collision.gameObject);
            TakeDamage(1);
        }
    }

    private void OnDestroy()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
    }
}
