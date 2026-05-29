using UnityEngine;

public class BurningEnemyBullet : MonoBehaviour
{
    private const int EnemyLayer = 3;
    private const int PlayerLayer = 6;
    private const int PlayerBulletLayer = 7;
    private const int EnemyBulletLayer = 9;

    [SerializeField] private int damage = 1;
    [SerializeField] private float burnTickDamage = 0.2f;
    [SerializeField] private int burnTickCount = 12;
    [SerializeField] private float burnTickInterval = 0.5f;
    [SerializeField] private float lifetime = 8f;

    private void Start()
    {
        Invoke(nameof(DestroyBullet), lifetime);
    }

    public void Configure(int configuredDamage, float configuredBurnTickDamage, int configuredBurnTickCount, float configuredBurnTickInterval)
    {
        damage = configuredDamage;
        burnTickDamage = configuredBurnTickDamage;
        burnTickCount = configuredBurnTickCount;
        burnTickInterval = configuredBurnTickInterval;
    }

    public void ApplyToPlayer(Player player)
    {
        if (player == null)
        {
            return;
        }

        player.TakeDamage(damage);
        player.ApplyBurn(burnTickDamage, burnTickCount, burnTickInterval);
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
