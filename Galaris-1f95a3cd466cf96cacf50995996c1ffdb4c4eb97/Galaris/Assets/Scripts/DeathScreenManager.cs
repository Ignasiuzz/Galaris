// DeathScreenManager.cs
using UnityEngine;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Find the existing ScoreManager object
        ScoreManager scoreManager = ScoreManager.Instance;

        if (scoreManager != null)
        {
            // Access the high score using scoreManager.HighScore
            highScoreText.text = "High Score: " + scoreManager.HighScore;

            // Access the score using scoreManager.Score
            scoreText.text = "Score: " + scoreManager.Score;

            // Ensure the high score is updated immediately
            scoreManager.ForceUpdateHighScore();
        }
    }
}
