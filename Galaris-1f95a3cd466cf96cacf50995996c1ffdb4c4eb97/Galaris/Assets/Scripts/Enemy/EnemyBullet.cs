using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Schedule the destruction of the bullet after 8 seconds
        Invoke("DestroyBullet", 4f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cancel the scheduled destruction since a collision occurred
        CancelInvoke("DestroyBullet");

        // Destroy the bullet
        Destroy(gameObject);
    }

    // Method to destroy the bullet
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
