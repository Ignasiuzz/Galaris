using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen instance;
    private const float PreLoadDisplayTime = 0.45f;
    private const float MinimumLoadDisplayTime = 1.1f;

    private Canvas canvas;
    private Image progressFill;
    private Text progressText;
    private Coroutine activeLoadRoutine;
    private bool isInitialized;

    public static void LoadScene(string sceneName)
    {
        EnsureInstance();
        instance.BeginLoad(sceneName);
    }

    public static void LoadScene(int buildIndex)
    {
        EnsureInstance();
        instance.BeginLoad(buildIndex);
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject loadingScreenObject = new GameObject("LoadingScreen");
        instance = loadingScreenObject.AddComponent<LoadingScreen>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUi();
        Hide();
    }

    private void InitializeUi()
    {
        if (isInitialized)
        {
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        GameObject panelObject = CreateUiObject("Panel", gameObject.transform);
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 1f);
        StretchToParent(panelObject.GetComponent<RectTransform>());

        GameObject titleObject = CreateUiObject("Title", panelObject.transform);
        Text titleText = titleObject.AddComponent<Text>();
        titleText.font = font;
        titleText.fontSize = 56;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.text = "Loading";

        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(700f, 120f);
        titleRect.anchoredPosition = new Vector2(0f, 90f);

        GameObject barBackgroundObject = CreateUiObject("BarBackground", panelObject.transform);
        Image barBackground = barBackgroundObject.AddComponent<Image>();
        barBackground.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform barBackgroundRect = barBackgroundObject.GetComponent<RectTransform>();
        barBackgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
        barBackgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
        barBackgroundRect.sizeDelta = new Vector2(700f, 36f);
        barBackgroundRect.anchoredPosition = new Vector2(0f, -10f);

        GameObject barFillObject = CreateUiObject("BarFill", barBackgroundObject.transform);
        progressFill = barFillObject.AddComponent<Image>();
        progressFill.color = Color.white;
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        progressFill.fillAmount = 0f;
        StretchToParent(barFillObject.GetComponent<RectTransform>());

        GameObject progressTextObject = CreateUiObject("ProgressText", panelObject.transform);
        progressText = progressTextObject.AddComponent<Text>();
        progressText.font = font;
        progressText.fontSize = 28;
        progressText.alignment = TextAnchor.MiddleCenter;
        progressText.color = Color.white;
        progressText.text = "0%";

        RectTransform progressRect = progressTextObject.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.5f, 0.5f);
        progressRect.anchorMax = new Vector2(0.5f, 0.5f);
        progressRect.sizeDelta = new Vector2(300f, 60f);
        progressRect.anchoredPosition = new Vector2(0f, -70f);
        isInitialized = true;
    }

    private void BeginLoad(string sceneName)
    {
        if (!EnsureUiReady())
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (activeLoadRoutine != null)
        {
            StopCoroutine(activeLoadRoutine);
        }

        activeLoadRoutine = StartCoroutine(LoadSceneByNameRoutine(sceneName));
    }

    private void BeginLoad(int buildIndex)
    {
        if (!EnsureUiReady())
        {
            SceneManager.LoadScene(buildIndex);
            return;
        }

        if (activeLoadRoutine != null)
        {
            StopCoroutine(activeLoadRoutine);
        }

        activeLoadRoutine = StartCoroutine(LoadSceneByIndexRoutine(buildIndex));
    }

    private IEnumerator LoadSceneByNameRoutine(string sceneName)
    {
        yield return ShowBeforeLoad();
        yield return LoadRoutine(SceneManager.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneByIndexRoutine(int buildIndex)
    {
        yield return ShowBeforeLoad();
        yield return LoadRoutine(SceneManager.LoadSceneAsync(buildIndex));
    }

    private IEnumerator ShowBeforeLoad()
    {
        Show();
        UpdateProgress(0f);
        yield return null;
        yield return new WaitForEndOfFrame();

        float elapsed = 0f;
        while (elapsed < PreLoadDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / PreLoadDisplayTime) * 0.15f;
            UpdateProgress(progress);
            yield return null;
        }
    }

    private IEnumerator LoadRoutine(AsyncOperation operation)
    {
        if (operation == null)
        {
            Hide();
            activeLoadRoutine = null;
            yield break;
        }

        operation.allowSceneActivation = false;
        float elapsed = 0f;

        while (operation.progress < 0.9f || elapsed < MinimumLoadDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsed / MinimumLoadDisplayTime);
            float progress = Mathf.Lerp(0.15f, 0.95f, Mathf.Min(sceneProgress, timeProgress));
            UpdateProgress(progress);
            yield return null;
        }

        UpdateProgress(1f);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        yield return null;
        Hide();
        activeLoadRoutine = null;
    }

    private void UpdateProgress(float progress)
    {
        if (progressFill != null)
        {
            progressFill.fillAmount = progress;
        }

        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }
    }

    private void Show()
    {
        if (!EnsureUiReady())
        {
            return;
        }

        canvas.enabled = true;
    }

    private void Hide()
    {
        if (canvas == null)
        {
            return;
        }

        canvas.enabled = false;
    }

    private bool EnsureUiReady()
    {
        if (canvas != null && isInitialized)
        {
            return true;
        }

        InitializeUi();
        return canvas != null && isInitialized;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
