using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public MovementJoyStick MovementJoyStick;
    public float PlayerSpeed = 10f;
    public float accelerationFactor = 2f;
    public float decelerationFactor = 2f;
    private Rigidbody2D rb;
    public int maxHealth = 10;
    public int currentHealth;
    public Health healthbar;
    public GameObject enemyBullet; // Reference to the enemy bullet object.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
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
            TakeDamage(1);

            // Optionally, add any additional logic or effects for when the player is hit by an enemy bullet.
            // For example, decrease the player's health, play a particle effect, etc.
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
    }

    void Die()
    {
        // You can add any death-related logic here, like showing the death screen or restarting the game.
        // For now, let's just print a message and load the death screen.
        Debug.Log("Player died!");
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        Movement();
        ApplySteering();
    }

    void Movement()
    {
        Vector2 targetVelocity = new Vector2(MovementJoyStick.joystickVec.x * PlayerSpeed, MovementJoyStick.joystickVec.y * PlayerSpeed);

        // Apply acceleration
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * accelerationFactor);

        // If joystick input is zero, apply deceleration
        if (MovementJoyStick.joystickVec == Vector2.zero)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * decelerationFactor);
        }
    }

    void ApplySteering()
    {
        if (rb.velocity.magnitude > 0.1f) // Check if the player is moving (you can adjust the threshold)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
