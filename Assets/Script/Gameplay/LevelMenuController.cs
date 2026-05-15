using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
    [Serializable]
    private struct LevelEntry
    {
        public string label;
        public string sceneName;
    }

    [Header("Text")]
    [SerializeField] private string menuTitle = "Forest Levels";
    [SerializeField] private string subtitle = "Choose a level";

    [Header("Levels")]
    [SerializeField] private LevelEntry[] levels =
    {
        new() { label = "Level 0", sceneName = "Level_0" },
        new() { label = "Level 1", sceneName = "level_1" },
        new() { label = "Level 2", sceneName = "level_2" },
        new() { label = "Level 3", sceneName = "level_3" },
        new() { label = "Level 4", sceneName = "Level_4" },
    };

    [Header("Colors")]
    [SerializeField] private Color backgroundColor = new(0.06f, 0.08f, 0.12f, 1f);
    [SerializeField] private Color panelColor = new(0.11f, 0.14f, 0.2f, 0.94f);
    [SerializeField] private Color accentColor = new(0.83f, 0.71f, 0.46f, 1f);
    [SerializeField] private Color buttonColor = new(0.24f, 0.34f, 0.22f, 0.95f);
    [SerializeField] private Color buttonTextColor = new(0.96f, 0.95f, 0.9f, 1f);

    private Font cachedFont;

    private void Awake()
    {
        Time.timeScale = 1f;
        EnsureCamera();
        EnsureEventSystem();
        BuildUi();
    }

    private void LoadLevel(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Level scene name is empty.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void BuildUi()
    {
        Canvas canvas = CreateCanvas();
        CreateBackground(canvas.transform);

        GameObject panel = CreatePanel("MenuPanel", canvas.transform, panelColor);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(680f, 720f);

        Text title = CreateText("Title", panel.transform, menuTitle, 58, FontStyle.Bold, accentColor);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(40f, -86f);
        titleRect.offsetMax = new Vector2(-40f, -20f);
        title.alignment = TextAnchor.MiddleCenter;

        Text subtitleText = CreateText("Subtitle", panel.transform, subtitle, 30, FontStyle.Normal, Color.white);
        RectTransform subtitleRect = subtitleText.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 1f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.pivot = new Vector2(0.5f, 1f);
        subtitleRect.offsetMin = new Vector2(40f, -136f);
        subtitleRect.offsetMax = new Vector2(-40f, -92f);
        subtitleText.alignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < levels.Length; i++)
        {
            LevelEntry entry = levels[i];
            float y = 182f - (i * 78f);
            CreateLevelButton(panel.transform, entry.label, entry.sceneName, y);
        }
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new("MenuCanvas");
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

    private void CreateBackground(Transform parent)
    {
        GameObject background = CreatePanel("Background", parent, backgroundColor);
        RectTransform rect = background.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void CreateLevelButton(Transform parent, string labelText, string sceneName, float y)
    {
        GameObject buttonObject = new(labelText);
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => LoadLevel(sceneName));

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(440f, 60f);

        Text label = CreateText("Label", buttonObject.transform, labelText, 30, FontStyle.Bold, buttonTextColor);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAnchor.MiddleCenter;
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

    private void EnsureCamera()
    {
        Camera sceneCamera = Camera.main;

        if (sceneCamera == null)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            sceneCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = backgroundColor;
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
