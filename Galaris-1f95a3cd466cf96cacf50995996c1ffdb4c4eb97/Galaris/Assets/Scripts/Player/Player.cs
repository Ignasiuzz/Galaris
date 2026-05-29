using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    public FloatingJoystick FloatingJoystick;
    public float PlayerSpeed = 10f;
    public float accelerationFactor = 2f;
    public float decelerationFactor = 2f;
    
    private Rigidbody2D rb;
    public float maxHealth;
    public float currentHealth;
    public Health healthbar;
    public GameObject enemyBullet; // Reference to the enemy bullet object.

    public GameObject playerBulletObject;
    public float bulletSpeed = 10f; // Adjust as needed.
    public LayerMask collisionLayers; // Set in the Inspector to specify which layers should trigger deletion;
    public GameObject selectedEnemy; // Reference to the manually selected enemy.
    public GameObject[] enemies;
    public Transform enemyTransform;
    public Transform EnemyBulletLocation;
    public Animator PlayerAnimator;
    private Rigidbody2D PlayerRigidbody;
    private Collider2D PlayerCollider;
    private SpriteRenderer playerSpriteRenderer;
    public Health healthBar;

    private float enemyDamage = 1f;
    
    //sound
    [SerializeField] private AudioSource ShootSoundEffect;
    [SerializeField] private AudioSource DeathSoundEffect;

    private PlayableArea playableArea; // Reference to the PlayableArea script
    private Quaternion initialRotation;
    private bool isDead = false;

    public float shootCooldown = 0.5f; // The time between shots
    private float lastShootTime = 0f; // The time of the last shot
    private float movementSpeedMultiplier = 1f;
    private Coroutine movementSlowRoutine;
    private Coroutine burnRoutine;
    private bool isMovementSlowed;
    private bool isBurning;
    private bool burnBlinkVisible;
    private Color defaultPlayerColor = Color.white;
    private static readonly Color FrozenPlayerColor = new Color(0.55f, 0.75f, 1f, 0.8f);
    private static readonly Color BurningPlayerColor = new Color(1f, 0.55f, 0.2f, 0.95f);

    void Start()
    {
        maxHealth = 10.0f;
        initialRotation = transform.rotation;
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
        rb = GetComponent<Rigidbody2D>();
        RefreshSceneReferences();
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        healthbar.SetMaxHealth(maxHealth);
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<Collider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            defaultPlayerColor = playerSpriteRenderer.color;
        }
        UpgradeMenu.instance?.ApplyCurrentUpgradesToPlayer(true);
    }

    void Update()
    {
        if (isDead)
        {
            return; // Don't perform any actions if the enemy is dead.
        }

        if (currentHealth <= 0)
        {
            Die();
        }
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBulletClone"))
        {
            Debug.Log("Enemy bullet collided with the player");

            FreezingEnemyBullet freezingBullet = collision.gameObject.GetComponent<FreezingEnemyBullet>();
            if (freezingBullet != null)
            {
                freezingBullet.ApplyToPlayer(this);
            }
            else
            {
                BurningEnemyBullet burningBullet = collision.gameObject.GetComponent<BurningEnemyBullet>();
                if (burningBullet != null)
                {
                    burningBullet.ApplyToPlayer(this);
                }
                else
                {
                    TakeDamage(enemyDamage);
                }
            }

            Destroy(collision.gameObject);

            // Optionally, add any additional logic or effects for when the player is hit by an enemy bullet.
            // For example, decrease the player's health, play a particle effect, etc.

            // Reset the player's rotation to the initial rotation.
            transform.rotation = initialRotation;
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        healthbar.SetHealth(currentHealth);
    }

    public void ApplyMovementSlow(float speedMultiplier, float duration)
    {
        float clampedMultiplier = Mathf.Clamp(speedMultiplier, 0.05f, 1f);

        if (movementSlowRoutine != null)
        {
            StopCoroutine(movementSlowRoutine);
        }

        movementSlowRoutine = StartCoroutine(ApplyMovementSlowRoutine(clampedMultiplier, duration));
    }

    public void ApplyBurn(float tickDamage, int tickCount, float tickInterval)
    {
        float clampedTickDamage = Mathf.Max(0f, tickDamage);
        int clampedTickCount = Mathf.Max(1, tickCount);
        float clampedTickInterval = Mathf.Max(0.05f, tickInterval);

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
        }

        isBurning = false;
        burnBlinkVisible = false;
        burnRoutine = StartCoroutine(ApplyBurnRoutine(clampedTickDamage, clampedTickCount, clampedTickInterval));
    }

    void Die()
    {
        // You can add any death-related logic here, like showing the death screen or restarting the game.
        // For now, let's just print a message and load the death screen.
        if (!isDead) isDead = true;
        SfxLimiter.TryPlay(DeathSoundEffect, "player_death", 0.5f, 1, 1f);
        PlayerAnimator.SetTrigger("Death");
        PlayerSpeed = 0f;
        healthBar.gameObject.SetActive(false);
        PlayerRigidbody.velocity = Vector2.zero;
        PlayerRigidbody.angularVelocity = 0f;
        Debug.Log("Player died!");
    }

    public void OnDeathAnimationEnd()
    {
        Debug.Log("Death animation ended");
        Destroy(gameObject);
        SceneManager.LoadScene(2);
    }

    void FixedUpdate()
    {
        Movement();
        ApplySteering();
    }

    private void RefreshSceneReferences()
    {
        if (FloatingJoystick == null)
        {
            FloatingJoystick = FindObjectOfType<FloatingJoystick>();
        }

        if (healthBar == null)
        {
            healthBar = FindObjectOfType<Health>();
        }

        if (healthbar == null)
        {
            healthbar = FindObjectOfType<Health>();
        }
    }

    void Movement()
    {
        if (FloatingJoystick == null)
        {
            RefreshSceneReferences();
        }

        if (FloatingJoystick == null)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * decelerationFactor);
            return;
        }

        Vector2 targetVelocity = new Vector2(FloatingJoystick.LHorizontal * PlayerSpeed * movementSpeedMultiplier, FloatingJoystick.LVertical * PlayerSpeed * movementSpeedMultiplier);

        // Apply acceleration
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * accelerationFactor);

        // If joystick input is zero, apply deceleration
        if (FloatingJoystick.Linput == Vector2.zero)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }

    private IEnumerator ApplyMovementSlowRoutine(float speedMultiplier, float duration)
    {
        movementSpeedMultiplier = speedMultiplier;
        isMovementSlowed = true;
        UpdatePlayerStatusVisual();
        yield return new WaitForSeconds(duration);
        movementSpeedMultiplier = 1f;
        isMovementSlowed = false;
        UpdatePlayerStatusVisual();
        movementSlowRoutine = null;
    }

    private IEnumerator ApplyBurnRoutine(float tickDamage, int tickCount, float tickInterval)
    {
        isBurning = true;

        for (int tickIndex = 0; tickIndex < tickCount; tickIndex++)
        {
            if (isDead)
            {
                break;
            }

            burnBlinkVisible = true;
            UpdatePlayerStatusVisual();
            TakeDamage(tickDamage);

            float blinkDuration = Mathf.Min(tickInterval * 0.5f, 0.12f);
            yield return new WaitForSeconds(blinkDuration);

            if (isDead)
            {
                break;
            }

            burnBlinkVisible = false;
            UpdatePlayerStatusVisual();

            float remainingDuration = tickInterval - blinkDuration;
            if (remainingDuration > 0f)
            {
                yield return new WaitForSeconds(remainingDuration);
            }
        }

        isBurning = false;
        burnBlinkVisible = false;
        UpdatePlayerStatusVisual();
        burnRoutine = null;
    }

    private void UpdatePlayerStatusVisual()
    {
        EnsurePlayerSpriteRenderer();
        if (playerSpriteRenderer == null)
        {
            return;
        }

        if (isBurning && burnBlinkVisible)
        {
            playerSpriteRenderer.color = BurningPlayerColor;
            return;
        }

        if (isMovementSlowed)
        {
            playerSpriteRenderer.color = FrozenPlayerColor;
            return;
        }

        playerSpriteRenderer.color = defaultPlayerColor;
    }

    private void EnsurePlayerSpriteRenderer()
    {
        if (playerSpriteRenderer != null)
        {
            return;
        }

        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            defaultPlayerColor = playerSpriteRenderer.color;
        }
    }

    void ApplySteering()
    {
        if (FloatingJoystick == null)
        {
            RefreshSceneReferences();
        }

        if (rb.velocity.magnitude > 0.1f) // Check if the player is moving
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        if (FloatingJoystick != null && FloatingJoystick.Rinput != Vector2.zero)
        {
            float angle = Mathf.Atan2(FloatingJoystick.RVertical, FloatingJoystick.RHorizontal) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Time.time - lastShootTime >= shootCooldown)
            {   
                SfxLimiter.TryPlay(ShootSoundEffect, "player_shoot", 0.04f, 4, 0.18f);
                // Call the Shoot function in the Player script here.
                GetComponent<Shooting>().Shoot();
                // Update the last shot time
                lastShootTime = Time.time;
                Debug.Log("Shooting");
            }
        }
    }

}
