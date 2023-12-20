using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public float spawnInterval;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnInfo[] enemyTypes;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private GameObject playerObject; // Reference to the player

    private int numberOfEnemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player object not assigned in the inspector.");
            return;
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true) // Infinite loop for continuous spawning
        {
            foreach (var enemyType in enemyTypes)
            {
                // Calculate spawn interval based on the score
                float adjustedSpawnInterval = enemyType.spawnInterval / (1 + ScoreManager.Instance.Score / 1000f);

                yield return new WaitForSeconds(adjustedSpawnInterval);

                // Calculate a random angle for the enemy spawn position
                float randomAngle = Random.Range(0f, 360f);

                // Calculate the spawn position based on the angle and spawn radius
                Vector3 spawnOffset = Quaternion.Euler(0, 0, randomAngle) * Vector3.right * spawnRadius;
                Vector3 spawnPosition = playerObject.transform.position + spawnOffset;

                // Spawn the enemy at the calculated position
                GameObject newEnemy = Instantiate(enemyType.enemyPrefab, spawnPosition, Quaternion.identity);

                // If the enemy has the Enemy2 script, set its Animator
                Enemy2 enemyScript = newEnemy.GetComponent<Enemy2>();
                if (enemyScript != null)
                {
                    Animator newEnemyAnimator = newEnemy.GetComponent<Animator>();
                    if (newEnemyAnimator != null)
                    {
                        newEnemyAnimator.runtimeAnimatorController = enemyScript.EnemyAnimator.runtimeAnimatorController;
                    }
                }

                // Increment the counter
                numberOfEnemiesSpawned++;

                // You can add more logic here if needed
            }
        }
    }
}
