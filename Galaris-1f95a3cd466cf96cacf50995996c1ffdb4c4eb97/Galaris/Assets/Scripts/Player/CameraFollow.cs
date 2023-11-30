using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    // Camera follow
    private void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Move the camera towards the target position with smoothing
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
