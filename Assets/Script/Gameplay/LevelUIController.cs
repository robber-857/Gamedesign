using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private string levelTitle = "Level 0  Tutorial";
    [SerializeField] private string levelObjective = "Press switches, ride lifts, reach the exit.";
    [SerializeField] private string controlsHint = "WASD Move   Space Jump";

    [Header("Colors")]
    [SerializeField] private Color panelColor = new(0.08f, 0.11f, 0.18f, 0.75f);
    [SerializeField] private Color accentColor = new(0.83f, 0.71f, 0.46f, 1f);
    [SerializeField] private Color buttonColor = new(0.24f, 0.34f, 0.22f, 0.95f);
    [SerializeField] private Color buttonTextColor = new(0.96f, 0.95f, 0.9f, 1f);

    private Font cachedFont;
    private Text completionText;

    private void Awake()
    {
        EnsureEventSystem();
        BuildUi();
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ShowLevelComplete()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
        }
    }

    private void BuildUi()
    {
        Canvas canvas = CreateCanvas();

        CreateHeaderPanel(canvas.transform);
        CreateRestartButton(canvas.transform);
        CreateControlsHint(canvas.transform);
        completionText = CreateCompletionLabel(canvas.transform);
        completionText.gameObject.SetActive(false);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new("LevelCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private void CreateHeaderPanel(Transform parent)
    {
        GameObject panel = CreatePanel("HeaderPanel", parent, panelColor);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(22f, -22f);
        rect.sizeDelta = new Vector2(620f, 140f);

        Text title = CreateText("Title", panel.transform, levelTitle, 48, FontStyle.Bold, accentColor);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(20f, -48f);
        titleRect.offsetMax = new Vector2(-20f, -8f);

        Text subtitle = CreateText("Subtitle", panel.transform, levelObjective, 40, FontStyle.Normal, Color.white);
        RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 0f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.offsetMin = new Vector2(20f, 16f);
        subtitleRect.offsetMax = new Vector2(-20f, -48f);
    }

    private void CreateRestartButton(Transform parent)
    {
        GameObject buttonObject = new("RestartButton");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(RestartLevel);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-22f, -22f);
        rect.sizeDelta = new Vector2(190f, 60f);

        Text label = CreateText("Label", buttonObject.transform, "Restart", 26, FontStyle.Bold, buttonTextColor);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAnchor.MiddleCenter;
    }

    private void CreateControlsHint(Transform parent)
    {
        GameObject panel = CreatePanel("ControlsHint", parent, panelColor);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 22f);
        rect.sizeDelta = new Vector2(500f, 60f);

        Text text = CreateText("HintText", panel.transform, controlsHint, 24, FontStyle.Bold, Color.white);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        text.alignment = TextAnchor.MiddleCenter;
    }

    private Text CreateCompletionLabel(Transform parent)
    {
        Text text = CreateText("CompletionText", parent, "Level Complete", 48, FontStyle.Bold, accentColor);
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 180f);
        rect.sizeDelta = new Vector2(500f, 80f);
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private Text CreateText(string name, Transform parent, string content, int fontSize, FontStyle fontStyle, Color color)
    {
        GameObject textObject = new(name);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = GetDefaultFont();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    private Font GetDefaultFont()
    {
        if (cachedFont != null)
        {
            return cachedFont;
        }

        cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return cachedFont;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
