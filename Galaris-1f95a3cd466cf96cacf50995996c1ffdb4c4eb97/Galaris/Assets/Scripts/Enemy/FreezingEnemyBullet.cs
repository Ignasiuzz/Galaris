using UnityEngine;

public class FreezingEnemyBullet : MonoBehaviour
{
    private const int EnemyLayer = 3;
    private const int PlayerLayer = 6;
    private const int PlayerBulletLayer = 7;
    private const int EnemyBulletLayer = 9;

    [SerializeField] private int damage = 1;
    [SerializeField] private float freezeSpeedMultiplier = 0.45f;
    [SerializeField] private float freezeDuration = 1.75f;
    [SerializeField] private float lifetime = 4f;

    private void Start()
    {
        Invoke(nameof(DestroyBullet), lifetime);
    }

    public void Configure(int configuredDamage, float configuredFreezeSpeedMultiplier, float configuredFreezeDuration)
    {
        damage = configuredDamage;
        freezeSpeedMultiplier = configuredFreezeSpeedMultiplier;
        freezeDuration = configuredFreezeDuration;
    }

    public void ApplyToPlayer(Player player)
    {
        if (player == null)
        {
            return;
        }

        player.TakeDamage(damage);
        player.ApplyMovementSlow(freezeSpeedMultiplier, freezeDuration);
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
            ApplyToPlayer(other.GetComponent<Player>());
        }

        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
