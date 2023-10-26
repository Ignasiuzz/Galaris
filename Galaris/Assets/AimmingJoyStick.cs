using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AimmingJoyStick : MonoBehaviour
{
    public int joystickId;
    public Transform player;  // Reference to the player's transform.
    public GameObject joystick2;
    public GameObject joystickBG2;
    private Vector2 joystickTouchPos2;
    private Vector2 joystickVec2;
    private Vector2 joystickOriginalPos2;
    private float joystickRadius2;
    private float shootCooldown = 0.1f; // The time between shots
    private float lastShootTime = 0f; // The time of the last shot

    // Create a public method to get the aiming direction.
    public Vector2 GetAimingDirection()
    {
        return joystickVec2;
    }

    void Start()
    {
        joystickOriginalPos2 = joystickBG2.transform.position;
        joystickRadius2 = joystickBG2.GetComponent<RectTransform>().sizeDelta.y / 4;
    }

    public void PointerDown()
    {
        joystick2.transform.position = Input.mousePosition;
        joystickBG2.transform.position = Input.mousePosition;
        joystickTouchPos2 = Input.mousePosition;
    }

    public void Drag(BaseEventData BaseEventData)
    {
        PointerEventData PointerEventData = BaseEventData as PointerEventData;
        Vector2 dragPos = PointerEventData.position;
        joystickVec2 = (dragPos - joystickTouchPos2).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos2);

        if (joystickDist < joystickRadius2)
        {
            joystick2.transform.position = joystickTouchPos2 + joystickVec2 * joystickDist;
        }
        else
        {
            joystick2.transform.position = joystickTouchPos2 + joystickVec2 * joystickRadius2;
        }

        // Check if enough time has passed since the last shot
        if (Time.time - lastShootTime >= shootCooldown)
        {
            // Call the Shoot function in the Player script here.
            player.GetComponent<Player>().Shoot();
            // Update the last shot time
            lastShootTime = Time.time;
        }
    }

    public void PointerUp()
    {
        joystickVec2 = Vector2.zero;
        joystick2.transform.position = joystickOriginalPos2;
        joystickBG2.transform.position = joystickOriginalPos2;
    }
}
