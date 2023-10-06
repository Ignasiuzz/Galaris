using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handeler : MonoBehaviour

{

    public MovementJoyStick MovementJoyStick;
    PlayerMovement PlayerMovement;

    void Awake()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = MovementJoyStick.joystickVec.x;
        inputVector.y = MovementJoyStick.joystickVec.y;

        PlayerMovement.SetInputVector(inputVector);
    }
}
