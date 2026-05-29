using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public Slider levelSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI killCountText;
    private Image fillImage;
    private Image panelImage;

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

        panelImage = GetComponent<Image>();
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

        bool isEndlessMode = LevelManager.Instance.IsEndlessMode();
        if (panelImage != null)
        {
            panelImage.enabled = !isEndlessMode;
        }

        if (isEndlessMode)
        {
            levelText.text = "Level ∞";

            if (killCountText != null)
            {
                killCountText.gameObject.SetActive(false);
            }

            if (levelSlider != null)
            {
                levelSlider.gameObject.SetActive(false);
            }

            return;
        }

        float progress = LevelManager.Instance.GetLevelProgress();
        levelText.text = $"Level {LevelManager.Instance.GetCurrentLevel()}";

        if (killCountText != null)
        {
            killCountText.gameObject.SetActive(true);
            killCountText.text = $"{LevelManager.Instance.GetEnemiesKilled()}/{LevelManager.Instance.GetEnemiesNeeded()}";
        }

        if (levelSlider != null)
        {
            levelSlider.gameObject.SetActive(true);
        }

        if (fillImage != null && levelSlider != null && levelSlider.gameObject.activeSelf)
        {
            Color fillColor = fillImage.color;
            fillColor.a = progress > 0 ? 1f : 0f;
            fillImage.color = fillColor;
        }
    }
}
