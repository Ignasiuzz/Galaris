using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Array of objects to spawn
    public float spawnDistanceMin = 5f; // Minimum spawn distance from the player
    public float spawnDistanceMax = 10f; // Maximum spawn distance from the player
    public float unloadDistance = 20f; // Distance at which objects will be set inactive
    public float spawnInterval = 5f; // Time interval for spawning objects

    [Header("Prefab Settings")]
    [SerializeField]
    private GameObject spawnableObjectPrefab; // Prefab containing sprite and collider

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnObjectsRoutine());
    }

    IEnumerator SpawnObjectsRoutine()
    {
        while (true)
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnInterval); // Adjust the time interval for spawning objects
        }
    }

    void SpawnObjects()
    {
        // Spawn new objects
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            // Generate random polar coordinates
            float randomDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
            float randomAngle = Random.Range(0f, 360f);

            // Convert polar coordinates to Cartesian coordinates
            float spawnX = player.position.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            float spawnY = player.position.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

            Vector3 randomPosition = new Vector3(spawnX, spawnY, 0f); // Assuming the game is in 2D

            // Instantiate the prefab directly for each new object
            GameObject spawnedObject = Instantiate(spawnableObjectPrefab, randomPosition, Quaternion.identity);

            // Set the spawned object to active
            spawnedObject.SetActive(true);

            // Assign the spawned object to the array
            objectsToSpawn[i] = spawnedObject;

            // Optional: You can perform additional setup on the spawned object if needed
        }
    }

    void Update()
    {
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            float distanceToPlayer = Vector3.Distance(objectsToSpawn[i].transform.position, player.position);

            // Set the object to inactive if it is beyond the specified distance
            if (distanceToPlayer > unloadDistance)
            {
                objectsToSpawn[i].SetActive(false);
            }
        }
    }
}
