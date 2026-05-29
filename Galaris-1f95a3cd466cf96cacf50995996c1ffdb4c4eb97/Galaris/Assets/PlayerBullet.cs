using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 4f;

    void Start()
    {
        Invoke(nameof(DestroyBullet), lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CancelInvoke(nameof(DestroyBullet));
        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
