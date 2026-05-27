// RestartMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    public void RestartGame()
    {
        // Reset the score when restarting
        ScoreManager.Instance.ResetScore();
        UpgradeMenu.instance.ResetUpgrades();
        LevelManager.Instance?.ResetLevel();

        SceneManager.LoadSceneAsync(1);
    }
}
