using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    public MovementJoyStick MovementJoyStick;
    public AimmingJoyStick AimmingJoyStick;
    public float PlayerSpeed = 10f;
    public float accelerationFactor = 2f;
    public float decelerationFactor = 2f;
    private Rigidbody2D rb;
    public int maxHealth = 10;
    public int currentHealth;
    public Health healthbar;
    public GameObject enemyBullet; // Reference to the enemy bullet object.
    public Transform firePoint;
    public Transform BackPoint;
    public GameObject playerBulletObject;
    public float bulletSpeed = 10f; // Adjust as needed.
    public LayerMask collisionLayers; // Set in the Inspector to specify which layers should trigger deletion;
    public GameObject selectedEnemy; // Reference to the manually selected enemy.
    public AimmingJoyStick aimingJoystick;
    public GameObject[] enemies;
    public Transform enemyTransform;
    public Transform EnemyBulletLocation;
    


    private PlayableArea playableArea; // Reference to the PlayableArea script
    private Quaternion initialRotation;


    void Start()
    {
        initialRotation = transform.rotation;
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
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

    void FixedUpdate()
    {
        Movement();
        ApplySteering();
    }
    public void Shoot()
    {
        {
                // Use the GetAimingDirection method from the AimmingJoyStick script.
                Vector2 shootingDirection = aimingJoystick.GetAimingDirection();

                GameObject bullet = Instantiate(playerBulletObject, firePoint.position, Quaternion.identity);

                // Calculate the angle based on the aiming direction.
                float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;

                // Set the bullet's rotation based on the calculated angle.
                bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // Set the tag for the bullet prefab in the Unity Inspector.
                bullet.tag = "PlayerBulletClone";

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = shootingDirection.normalized * bulletSpeed;
           
        }
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
        if (rb.velocity.magnitude > 0.1f) // Check if the player is moving
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        // Get the aiming direction from the AimmingJoyStick script.
        Vector2 aimingDirection = aimingJoystick.GetAimingDirection();

        if (aimingDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimingDirection.y, aimingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
