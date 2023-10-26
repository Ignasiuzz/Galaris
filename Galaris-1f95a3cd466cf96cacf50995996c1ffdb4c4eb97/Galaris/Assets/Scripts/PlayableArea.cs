using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableArea : MonoBehaviour
{
    public float width = 10f;
    public float height = 10f;

    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe rectangle in the Unity Editor to visualize the playable area.
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }
}