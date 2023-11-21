using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    // Joystick
    public FloatingJoystick FloatingJoystick;
    public float PlayerSpeed = 10f;
    public float accelerationFactor = 2f;
    public float decelerationFactor = 2f;

    //Health
    private Rigidbody2D rb;
    public int maxHealth = 10;
    public int currentHealth;
    public Health healthbar;

    public GameObject enemyBullet; // Reference to the enemy bullet object.
    public Transform BackPoint;
    public GameObject playerBulletObject;
    public LayerMask collisionLayers; // Set in the Inspector to specify which layers should trigger deletion;
    public GameObject selectedEnemy; // Reference to the manually selected enemy.

    public GameObject[] enemies;
    public Transform enemyTransform;
    public Transform EnemyBulletLocation;

    [SerializeField] Health healthBar;
    
    private PlayableArea playableArea; // Reference to the PlayableArea script
    private Quaternion initialRotation;

    private float shootCooldown = 0.5f; // The time between shots
    private float lastShootTime = 0f; // The time of the last shot

    void Start()
    {
        initialRotation = transform.rotation;
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<Health>();
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        healthbar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
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
            TakeDamage(1);

            // Optionally, add any additional logic or effects for when the player is hit by an enemy bullet.
            // For example, decrease the player's health, play a particle effect, etc.

            // Reset the player's rotation to the initial rotation.
            transform.rotation = initialRotation;
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        healthbar.SetHealth(currentHealth);
    }

    void Die()
    {
        // You can add any death-related logic here, like showing the death screen or restarting the game.
        // For now, let's just print a message and load the death screen.
        Debug.Log("Player died!");
        SceneManager.LoadScene(2);
        Destroy(gameObject);
    }

    void FixedUpdate()      //Issaukia joystick controll funkcijas
    {
        Movement();
        ApplySteering();
    }
    //RIP shooting
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

            //Sitas if veikai kai if(FloatingJoystick.Rinput != Vector2.zero), gauna koda is FloatingJoysticks.cs, kad aiming joystick veikia
            if (Time.time - lastShootTime >= shootCooldown)
                {
                    // Call the Shoot function in the Player script here.
                    GetComponent<Shooting_no_delete>().Shoot();
                    // Update the last shot time
                    lastShootTime = Time.time;
                    Debug.Log("Shooting");
                }
        }
    }
}
