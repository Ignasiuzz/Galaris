using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public MovementJoyStick MovementJoyStick;
    public float PlayerMaxSpeed = 8f;
    public float AccelerationSpeed = 1.2f;
    public float turnFactor = 3.5f;
    public float Speed = 2f;
    private Rigidbody2D rb;

    float velocityVsUp = 0;
    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
    }

    void ApplyEngineForce()
    {
        //Apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
        if (accelerationInput == 0&&steeringInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
        {
        rb.drag = 0;

        Vector2 engineForceVector = transform.up * Speed * AccelerationSpeed;
        //Apply force and pushes the car forward
        rb.AddForce(engineForceVector, ForceMode2D.Force);
        }
    }

    void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (rb.velocity.magnitude / 2);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor;

        //Apply steering by rotating the car object
        rb.rotation = rotationAngle;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}
