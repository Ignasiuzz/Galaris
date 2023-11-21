using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundPrefab; // The prefab of the background sprite
    public int gridWidth = 3; // Number of backgrounds in the x-axis
    public int gridHeight = 3; // Number of backgrounds in the y-axis
    public float backgroundSize = 10f; // The size of each background sprite

    private Camera mainCamera;
    private GameObject[,] backgrounds;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeBackgrounds();
    }

    void Update()
    {
        // Check if the camera has moved enough to reposition backgrounds
        RepositionBackgrounds();
    }

    void InitializeBackgrounds()
    {
        backgrounds = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 spawnPosition = new Vector3(x * backgroundSize, y * backgroundSize, 0f);
                backgrounds[x, y] = Instantiate(backgroundPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    void RepositionBackgrounds()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        float xOffset = Mathf.Floor(cameraPosition.x / backgroundSize) * backgroundSize;
        float yOffset = Mathf.Floor(cameraPosition.y / backgroundSize) * backgroundSize;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 newPosition = new Vector3(xOffset + x * backgroundSize, yOffset + y * backgroundSize, 0f);
                backgrounds[x, y].transform.position = newPosition;
            }
        }
    }
}
