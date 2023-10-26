using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int score = 0;
    private int highScore = 0;
    private string highScoreKey = "HighScore";

    void Start()
{
    // Load the high score from PlayerPrefs
    highScore = PlayerPrefs.GetInt(highScoreKey, 0);

    // Initialize the UI elements
    score = 0; // Set the initial score to zero
    UpdateScoreText();
    UpdateHighScoreText();
}


    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(highScoreKey, highScore);
            UpdateHighScoreText();
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore;
    }
}
