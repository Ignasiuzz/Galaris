using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private const int EnemyLayer = 3;
    private const int PlayerLayer = 6;
    private const int PlayerBulletLayer = 7;
    private const int EnemyBulletLayer = 9;

    void Start()
    {
        Invoke("DestroyBullet", 4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;
        if (otherLayer == EnemyLayer || otherLayer == EnemyBulletLayer || otherLayer == PlayerBulletLayer)
        {
            return;
        }

        CancelInvoke(nameof(DestroyBullet));

        if (otherLayer == PlayerLayer)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1f);
            }
        }

        Destroy(gameObject);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
