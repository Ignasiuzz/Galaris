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

    private ScoreManager scoreManager;
    private int numberOfEnemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        RefreshSceneReferences();

        if (playerObject == null)
        {
            Debug.LogWarning("EnemySpawner: Player object not found yet. Spawning will wait until a player exists.");
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private void RefreshSceneReferences()
    {
        if (playerObject == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                playerObject = player.gameObject;
            }
            else
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
            }
        }

        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
            if (scoreManager == null)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true) // Infinite loop for continuous spawning
        {
            RefreshSceneReferences();

            if (playerObject == null)
            {
                yield return null;
                continue;
            }

            if (enemyTypes == null || enemyTypes.Length == 0)
            {
                yield return null;
                continue;
            }

            foreach (var enemyType in enemyTypes)
            {
                if (enemyType == null || enemyType.enemyPrefab == null)
                {
                    continue;
                }

                // Calculate spawn interval based on the score
                int score = scoreManager != null ? scoreManager.Score : 0;
                float adjustedSpawnInterval = enemyType.spawnInterval / (1 + score / 1000f);

                yield return new WaitForSeconds(adjustedSpawnInterval);

                RefreshSceneReferences();

                if (playerObject == null)
                {
                    continue;
                }

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
                    if (newEnemyAnimator != null && enemyScript.EnemyAnimator != null)
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
