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

    void Start()
    {
        maxHealth = 10.0f;
        initialRotation = transform.rotation;
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar = FindObjectOfType<Health>();
        healthbar = FindObjectOfType<Health>();
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        healthbar.SetMaxHealth(maxHealth);
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<Collider2D>();
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

            // Destroy the enemyBullet
            Destroy(collision.gameObject);
            TakeDamage(enemyDamage);

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

    void Die()
    {
        // You can add any death-related logic here, like showing the death screen or restarting the game.
        // For now, let's just print a message and load the death screen.
        if (!isDead) isDead = true;
        DeathSoundEffect.Play();
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
    void Movement()
    {
        Vector2 targetVelocity = new Vector2(FloatingJoystick.LHorizontal * PlayerSpeed, FloatingJoystick.LVertical * PlayerSpeed);

        // Apply acceleration
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * accelerationFactor);

        // If joystick input is zero, apply deceleration
        if (FloatingJoystick.Linput == Vector2.zero)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }

    void ApplySteering()
    {
        if (rb.velocity.magnitude > 0.1f) // Check if the player is moving
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        if (FloatingJoystick.Rinput != Vector2.zero)
        {
            float angle = Mathf.Atan2(FloatingJoystick.RVertical, FloatingJoystick.RHorizontal) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Time.time - lastShootTime >= shootCooldown)
            {   
                ShootSoundEffect.Play();
                // Call the Shoot function in the Player script here.
                GetComponent<Shooting>().Shoot();
                // Update the last shot time
                lastShootTime = Time.time;
                Debug.Log("Shooting");
            }
        }
    }

}
