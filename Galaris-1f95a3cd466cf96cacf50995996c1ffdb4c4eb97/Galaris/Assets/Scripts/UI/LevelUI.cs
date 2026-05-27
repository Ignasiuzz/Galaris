using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public Slider levelSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI killCountText;
    private Image fillImage;

    private float targetValue = 0f;
    private float smoothSpeed = 5f;

    void Start()
    {
        if (levelSlider == null || levelText == null || killCountText == null)
        {
            Debug.LogError("LevelUI: Missing UI references!");
            return;
        }

        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelUI: LevelManager.Instance is null! Make sure LevelManager exists in the scene.");
            return;
        }

        fillImage = levelSlider.fillRect.GetComponent<Image>();
        UpdateLevelDisplay();
    }

    void Update()
    {
        UpdateLevelDisplay();

        if (LevelManager.Instance != null)
        {
            targetValue = LevelManager.Instance.GetLevelProgress();
            levelSlider.value = Mathf.Lerp(levelSlider.value, targetValue, smoothSpeed * Time.deltaTime);
        }
    }

    private void UpdateLevelDisplay()
    {
        if (LevelManager.Instance == null)
            return;

        float progress = LevelManager.Instance.GetLevelProgress();
        levelText.text = $"Level {LevelManager.Instance.GetCurrentLevel()}";
        killCountText.text = $"{LevelManager.Instance.GetEnemiesKilled()}/{LevelManager.Instance.GetEnemiesNeeded()}";

        if (fillImage != null)
        {
            Color fillColor = fillImage.color;
            fillColor.a = progress > 0 ? 1f : 0f;
            fillImage.color = fillColor;
        }
    }
}

