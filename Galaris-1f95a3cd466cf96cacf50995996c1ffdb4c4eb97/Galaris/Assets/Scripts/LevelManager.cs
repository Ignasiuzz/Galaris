using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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
        if (scene.name != "Game" && scene.name != "Level2")
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
        currentLevel++;
        enemiesKilled = 0;
        CalculateEnemiesNeeded();
        Debug.Log($"Level up! Now on Level {currentLevel}");

        if (currentLevel == 2)
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
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
