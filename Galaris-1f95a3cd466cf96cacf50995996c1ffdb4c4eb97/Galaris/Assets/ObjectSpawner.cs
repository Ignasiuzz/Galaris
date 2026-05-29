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
    private class PooledObject
    {
        public GameObject Instance;
        public int PrefabIndex;
        public float Radius;
    }

    public int poolSize = 30; // Number of objects in the pool
    public float spawnDistanceMin = 5f;
    public float spawnDistanceMax = 10f;
    public float unloadDistance = 20f;
    public float spawnInterval = 1f;
    public int objectsToSpawn = 5; // Number of objects to spawn each time
    public float minDistanceBetweenObjects = 3f; // Extra padding between spawned objects
    public List<SpawnableObject> spawnableObjects = new List<SpawnableObject>(); // List of spawnable prefabs

    [Header("Player Settings")]
    [SerializeField]
    private Transform playerTransform; // Player transform

    private List<PooledObject> objectPool = new List<PooledObject>();

    void Start()
    {
        if (spawnableObjects.Count == 0)
        {
            Debug.LogError("ObjectSpawner has no spawnable objects configured.");
            enabled = false;
            return;
        }

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (playerTransform == null)
        {
            Debug.LogError("ObjectSpawner could not find the player transform.");
            enabled = false;
            return;
        }

        InitializeObjectPool();
        StartCoroutine(SpawnObjectsRoutine());
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            objectPool.Add(CreatePooledObject(GetRandomPrefabIndex()));
        }
    }

    IEnumerator SpawnObjectsRoutine()
    {
        while (true)
        {
            DespawnDistantObjects();
            SpawnObjects(objectsToSpawn); // Specify the number of objects to spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjects(int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            int prefabIndex = GetRandomPrefabIndex();
            PooledObject pooledObject = GetAvailableObject();
            if (pooledObject == null)
            {
                break;
            }

            EnsurePrefabType(pooledObject, prefabIndex);
            float candidateRadius = pooledObject.Radius;

            for (int attempt = 0; attempt < objectPool.Count; attempt++)
            {
                float randomDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
                float randomAngle = Random.Range(0f, 360f);

                Vector3 randomOffset = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)) * randomDistance;
                Vector3 randomPosition = playerTransform.position + randomOffset;

                float minProximity = 2f + candidateRadius;
                if (Vector3.Distance(randomPosition, playerTransform.position) < minProximity)
                {
                    continue;
                }

                bool overlap = false;
                for (int k = 0; k < objectPool.Count; k++)
                {
                    if (!objectPool[k].Instance.activeInHierarchy)
                    {
                        continue;
                    }

                    float distanceBetweenObjects = Vector3.Distance(randomPosition, objectPool[k].Instance.transform.position);
                    float requiredSpacing = candidateRadius + objectPool[k].Radius + minDistanceBetweenObjects;
                    if (distanceBetweenObjects < requiredSpacing)
                    {
                        overlap = true;
                        break;
                    }
                }

                if (overlap)
                {
                    continue;
                }

                pooledObject.Instance.transform.position = randomPosition;
                pooledObject.Instance.SetActive(true);
                break;
            }
        }
    }

    PooledObject CreatePooledObject(int prefabIndex)
    {
        GameObject obj = Instantiate(spawnableObjects[prefabIndex].prefab, Vector3.zero, Quaternion.identity);
        obj.SetActive(false);

        return new PooledObject
        {
            Instance = obj,
            PrefabIndex = prefabIndex,
            Radius = GetObjectRadius(obj)
        };
    }

    void EnsurePrefabType(PooledObject pooledObject, int prefabIndex)
    {
        if (pooledObject.PrefabIndex == prefabIndex)
        {
            return;
        }

        Destroy(pooledObject.Instance);
        pooledObject.Instance = Instantiate(spawnableObjects[prefabIndex].prefab, Vector3.zero, Quaternion.identity);
        pooledObject.Instance.SetActive(false);
        pooledObject.PrefabIndex = prefabIndex;
        pooledObject.Radius = GetObjectRadius(pooledObject.Instance);
    }

    PooledObject GetAvailableObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].Instance.activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        PooledObject farthestObject = null;
        float farthestDistance = spawnDistanceMax;

        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].Instance.activeInHierarchy)
            {
                continue;
            }

            float distanceToPlayer = Vector3.Distance(objectPool[i].Instance.transform.position, playerTransform.position);
            if (distanceToPlayer > farthestDistance)
            {
                farthestDistance = distanceToPlayer;
                farthestObject = objectPool[i];
            }
        }

        if (farthestObject != null)
        {
            farthestObject.Instance.SetActive(false);
        }

        return farthestObject;
    }

    int GetRandomPrefabIndex()
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
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            SpawnableObject spawnableObject = spawnableObjects[i];
            cumulativeProbability += spawnableObject.spawnProbability;

            if (randomValue <= cumulativeProbability)
            {
                return i;
            }
        }

        return 0;
    }

    float GetObjectRadius(GameObject obj)
    {
        CircleCollider2D circleCollider = obj.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            Vector3 scale = obj.transform.lossyScale;
            float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
            return circleCollider.radius * maxScale;
        }

        Collider2D collider2D = obj.GetComponent<Collider2D>();
        if (collider2D != null)
        {
            return Mathf.Max(collider2D.bounds.extents.x, collider2D.bounds.extents.y);
        }

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return Mathf.Max(spriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y);
        }

        return minDistanceBetweenObjects * 0.5f;
    }

    void Update()
    {
        DespawnDistantObjects();
    }

    void DespawnDistantObjects()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            float distanceToPlayer = Vector3.Distance(objectPool[i].Instance.transform.position, playerTransform.position);

            if (distanceToPlayer > unloadDistance)
            {
                objectPool[i].Instance.SetActive(false);
            }
        }
    }
}
