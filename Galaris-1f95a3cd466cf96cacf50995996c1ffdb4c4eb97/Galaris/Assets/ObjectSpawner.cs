using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    public float spawnProbability = 1.0f;
}

public class ObjectSpawner : MonoBehaviour
{
    public int poolSize = 10; // Number of objects in the pool
    public float spawnDistanceMin = 5f;
    public float spawnDistanceMax = 10f;
    public float unloadDistance = 20f;
    public float spawnInterval = 5f;
    public int objectsToSpawn = 3; // Number of objects to spawn each time
    public float minDistanceBetweenObjects = 1f; // Minimum distance between spawned objects
    public List<SpawnableObject> spawnableObjects = new List<SpawnableObject>(); // List of spawnable prefabs

    [Header("Player Settings")]
    [SerializeField]
    private Transform playerTransform; // Player transform

    private List<GameObject> objectPool = new List<GameObject>();

    void Start()
    {
        InitializeObjectPool();
        StartCoroutine(SpawnObjectsRoutine());
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(spawnableObjects[0].prefab, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    IEnumerator SpawnObjectsRoutine()
    {
        while (true)
        {
            SpawnObjects(objectsToSpawn); // Specify the number of objects to spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjects(int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            for (int j = 0; j < objectPool.Count; j++)
            {
                if (!objectPool[j].activeInHierarchy)
                {
                    float randomDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
                    float randomAngle = Random.Range(0f, 360f);

                    // Calculate the random position around the player in the x and y plane
                    Vector3 randomOffset = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)) * randomDistance;
                    Vector3 randomPosition = playerTransform.position + randomOffset;

                    // Check if the distance to the player is greater than a minimum proximity
                    float minProximity = 2f; // Adjust this value based on your preferences
                    if (Vector3.Distance(randomPosition, playerTransform.position) < minProximity)
                    {
                        continue; // Skip this iteration and try again
                    }

                    // Check if the new object overlaps with existing objects
                    bool overlap = false;
                    for (int k = 0; k < objectPool.Count; k++)
                    {
                        if (objectPool[k].activeInHierarchy)
                        {
                            float distanceBetweenObjects = Vector3.Distance(randomPosition, objectPool[k].transform.position);

                            if (distanceBetweenObjects < minDistanceBetweenObjects)
                            {
                                overlap = true;
                                break;
                            }
                        }
                    }

                    if (overlap)
                    {
                        continue; // Skip this iteration and try again
                    }

                    // Randomly select a spawnable prefab
                    GameObject selectedPrefab = GetRandomPrefab();

                    // Spawn the selected prefab
                    objectPool[j] = Instantiate(selectedPrefab, randomPosition, Quaternion.identity);
                    objectPool[j].SetActive(true);

                    // Optional: You can perform additional setup on the spawned object if needed
                    break; // Break out of the loop after spawning one object
                }
            }
        }
    }

    GameObject GetRandomPrefab()
    {
        float totalProbability = 0f;

        // Calculate the total probability of all spawnable objects
        foreach (var spawnableObject in spawnableObjects)
        {
            totalProbability += spawnableObject.spawnProbability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        // Choose the prefab based on the random value and spawn probabilities
        foreach (var spawnableObject in spawnableObjects)
        {
            cumulativeProbability += spawnableObject.spawnProbability;

            if (randomValue <= cumulativeProbability)
            {
                return spawnableObject.prefab;
            }
        }

        // If for some reason the loop did not return a prefab, return the first one
        return spawnableObjects[0].prefab;
    }

    void Update()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            float distanceToPlayer = Vector3.Distance(objectPool[i].transform.position, playerTransform.position);

            if (distanceToPlayer > unloadDistance)
            {
                objectPool[i].SetActive(false);
            }
        }
    }
}
