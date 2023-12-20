using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    UpgradeMenu UP;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI highScoreText;

    private int score = 0;
    private int highScore = 0;
    private string highScoreKey = "HighScore";

    // Make highScore accessible to other scripts
    public int HighScore { get { return highScore; } }

    // Make score accessible to other scripts
    public int Score { get { return score; } }

    // Static variable to store the most recent score
    private static int lastScore;

    // Constants
    private const int PointsPerUpgrade = 10;

    // Singleton instance
    public static ScoreManager Instance;

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UP = FindObjectOfType<UpgradeMenu>();
    }

    void Start()
    {
        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);

        // Set the initial score to zero
        score = 0;

        // Initialize the UI elements
        UpdateHighScoreText();
        SetScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        UP.UpgradePoints_ += points / PointsPerUpgrade;

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
            UpdateHighScoreText();
        }

        SetScoreText();
        UP.SetUPText();
    }

    // Method to set the last score (called before loading DeathScreen)
    public void SetLastScore()
    {
        lastScore = score;
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, highScore);
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(highScoreKey);
        highScore = 0;
        UpdateHighScoreText();
    }

    // Add this method to ensure the high score is updated immediately
    public void ForceUpdateHighScore()
    {
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        UpdateHighScoreText();
    }

    // Add this method to reset the score to 0
    public void ResetScore()
    {
        UP.UpgradePoints_ = 0;
        score = 0;
        SetScoreText();
        UP.SetUPText();
    }

    // Method to load the DeathScreen scene
    public void LoadDeathScreen()
    {
        SetLastScore(); // Set the last score before transitioning
        SceneManager.LoadScene("DeathScreen");
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    void SetScoreText()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score;
        }
    }
}
