using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handeler : MonoBehaviour
{
    MovementV2 MovementV2;

    void Start()
    {
        MovementV2 = GetComponent<MovementV2>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        MovementV2.SetInputVector(inputVector);
    }
}
