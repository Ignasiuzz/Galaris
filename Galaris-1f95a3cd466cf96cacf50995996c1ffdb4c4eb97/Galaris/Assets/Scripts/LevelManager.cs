using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private readonly string[] levelScenes = { "Game", "Level2", "Level3" };

    [SerializeField] private int[] enemiesNeededPerLevel = { 20, 30, 40 };

    public int currentLevel = 1;
    private int enemiesKilled = 0;
    private int enemiesNeededForLevelUp;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CalculateEnemiesNeeded();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsGameplayScene(scene.name))
        {
            Destroy(gameObject);
            Instance = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CalculateEnemiesNeeded()
    {
        int levelIndex = currentLevel - 1;
        if (levelIndex >= 0 && levelIndex < enemiesNeededPerLevel.Length)
        {
            enemiesNeededForLevelUp = Mathf.Max(1, enemiesNeededPerLevel[levelIndex]);
            return;
        }

        enemiesNeededForLevelUp = 10 + (currentLevel * 10);
    }

    public void RegisterEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemy killed! {enemiesKilled}/{enemiesNeededForLevelUp}");

        if (enemiesKilled >= enemiesNeededForLevelUp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (currentLevel >= levelScenes.Length)
        {
            enemiesKilled = 0;
            CalculateEnemiesNeeded();
            Debug.Log("Already on the final level.");
            return;
        }

        currentLevel++;
        enemiesKilled = 0;
        CalculateEnemiesNeeded();
        Debug.Log($"Level up! Now on Level {currentLevel}");
        UpgradeMenu.instance?.CarryCurrentPlayerHealth();
        LoadingScreen.LoadScene(levelScenes[currentLevel - 1]);
    }

    private bool IsGameplayScene(string sceneName)
    {
        foreach (string levelScene in levelScenes)
        {
            if (sceneName == levelScene)
            {
                return true;
            }
        }

        return false;
    }

    public float GetLevelProgress()
    {
        return (float)enemiesKilled / enemiesNeededForLevelUp;
    }

    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }

    public int GetEnemiesNeeded()
    {
        return enemiesNeededForLevelUp;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void ResetLevel()
    {
        currentLevel = 1;
        enemiesKilled = 0;
        CalculateEnemiesNeeded();
        Debug.Log("Level reset to 1");
    }
}
