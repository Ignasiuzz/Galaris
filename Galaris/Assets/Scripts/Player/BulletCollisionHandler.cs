using UnityEngine;

public class BulletCollisionHandler : MonoBehaviour
{
    private LayerMask collisionLayers;
    private PlayableArea playableArea;

    public void Initialize(LayerMask layers, PlayableArea area)
    {
        collisionLayers = layers;
        playableArea = area;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the bullet collided with objects on specified layers.
        if (collisionLayers == (collisionLayers | (1 << collision.gameObject.layer)))
        {
            // Bullet collided with a valid object, delete it.
            Destroy(gameObject);
        }
    }
}
