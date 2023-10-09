using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
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
