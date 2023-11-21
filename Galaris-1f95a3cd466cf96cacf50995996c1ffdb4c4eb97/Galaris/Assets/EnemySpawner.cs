using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float enemySpawnInterval = 15f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private GameObject playerObject; // Reference to the player

    // Start is called before the first frame update
    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player object not assigned in the inspector.");
            return;
        }

        StartCoroutine(SpawnEnemyCoroutine(enemySpawnInterval, enemyPrefab));
    }

    private IEnumerator SpawnEnemyCoroutine(float interval, GameObject enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // Check if you want to stop spawning enemies (e.g., based on a condition)
            // if (stopSpawningCondition) break;

            // Calculate a random angle for the enemy spawn position
            float randomAngle = Random.Range(0f, 360f);

            // Calculate the spawn position based on the angle and spawn radius
            Vector3 spawnOffset = Quaternion.Euler(0, 0, randomAngle) * Vector3.right * spawnRadius;
            Vector3 spawnPosition = playerObject.transform.position + spawnOffset;

            // Spawn the enemy at the calculated position
            GameObject newEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);

            // You can add more logic here if needed

            // Example: Stop spawning after a certain number of enemies
            // if (numberOfEnemiesSpawned >= maxEnemiesToSpawn) break;
        }
    }
}
