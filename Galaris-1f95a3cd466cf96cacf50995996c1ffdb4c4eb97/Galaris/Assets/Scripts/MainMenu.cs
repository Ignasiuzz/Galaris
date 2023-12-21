using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        ScoreManager.Instance.ResetScore();
        UpgradeMenu.instance.ResetUpgrades();

        SceneManager.LoadSceneAsync(0);
    }
}