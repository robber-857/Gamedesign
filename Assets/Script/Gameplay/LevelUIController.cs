using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private string levelTitle = "Level 0 Tutorial";
    [SerializeField] private string levelObjective = "";
    [SerializeField] private string controlsHint = "";
    [SerializeField] private string completionMessage = "Congratulations!";
    [SerializeField] private string failureTitle = "Take a breath";
    [SerializeField] private string failureMessage = "Choose where you want to try again.";
    [SerializeField] private string menuSceneName = "LevelMenu";

    [Header("Colors")]
    [SerializeField] private Color panelColor = new(0.08f, 0.11f, 0.18f, 0.75f);
    [SerializeField] private Color overlayColor = new(0.03f, 0.04f, 0.06f, 0.86f);
    [SerializeField] private Color accentColor = new(0.83f, 0.71f, 0.46f, 1f);
    [SerializeField] private Color dangerColor = new(0.84f, 0.22f, 0.16f, 1f);
    [SerializeField] private Color buttonColor = new(0.24f, 0.34f, 0.22f, 0.95f);
    [SerializeField] private Color buttonTextColor = new(0.96f, 0.95f, 0.9f, 1f);

    private Font cachedFont;
    private Text titleText;
    private Text objectiveText;
    private Text controlsHintText;
    private Text completionText;
    private Text failureTitleText;
    private Text failureMessageText;
    private GameObject failureCheckpointButton;
    private GameObject failureMenuButton;
    private GameObject failureOverlay;
    private GameObject menuOverlay;

    public static LevelUIController FindOrCreate()
    {
        LevelUIController existing = FindFirstObjectByType<LevelUIController>();

        if (existing != null)
        {
            return existing;
        }

        GameObject controllerObject = new("LevelUIController");
        return controllerObject.AddComponent<LevelUIController>();
    }

    private void Awake()
    {
        Time.timeScale = 1f;
        EnsureEventSystem();
        BuildUi();
    }

    public void RestartLevel()
    {
        CheckpointManager.ClearCurrentSceneCheckpoint();
        ReloadCurrentScene();
    }

    public void RestartButtonPressed()
    {
        RestartLevel();
    }

    public void ToggleMenu()
    {
        if (menuOverlay == null)
        {
            return;
        }

        if (menuOverlay.activeSelf)
        {
            HideMenu();
            return;
        }

        ShowMenu();
    }

    public void ShowMenu()
    {
        if (menuOverlay == null || failureOverlay != null && failureOverlay.activeSelf)
        {
            return;
        }

        menuOverlay.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideMenu()
    {
        if (menuOverlay != null)
        {
            menuOverlay.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void RestartFromLastCheckpoint()
    {
        if (!CheckpointManager.HasCheckpointForCurrentScene())
        {
            RestartLevel();
            return;
        }

        ReloadCurrentScene();
    }

    public void RestartFailureAttempt()
    {
        RestartFromLastCheckpoint();
    }

    private void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(menuSceneName))
        {
            Debug.LogWarning("Menu scene name is empty.");
            return;
        }

        SceneManager.LoadScene(menuSceneName);
    }

    public void ShowLevelComplete()
    {
        if (completionText != null)
        {
            completionText.text = completionMessage;
            completionText.gameObject.SetActive(true);
        }
    }

    public void ShowFailure()
    {
        ShowFailure(failureTitle, failureMessage);
    }

    public void ShowFailure(string title, string message)
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }

        if (menuOverlay != null)
        {
            menuOverlay.SetActive(false);
        }

        if (failureTitleText != null)
        {
            failureTitleText.text = GetSoftFailureTitle(title);
        }

        if (failureMessageText != null)
        {
            failureMessageText.text = message;
        }

        RefreshFailureButtons();

        if (failureOverlay != null)
        {
            failureOverlay.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Configure(string title, string objective, string controls, string completion)
    {
        levelTitle = title;
        levelObjective = objective;
        controlsHint = controls;
        completionMessage = completion;
        RefreshUiText();
    }

    private void BuildUi()
    {
        Canvas canvas = CreateCanvas();

        CreateHeaderPanel(canvas.transform);
        CreateMenuButton(canvas.transform);
        CreateControlsHint(canvas.transform);
        completionText = CreateCompletionLabel(canvas.transform);
        menuOverlay = CreateMenuOverlay(canvas.transform);
        failureOverlay = CreateFailureOverlay(canvas.transform);
        RefreshUiText();
        completionText.gameObject.SetActive(false);
        menuOverlay.SetActive(false);
        failureOverlay.SetActive(false);
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
        bool hasObjective = !string.IsNullOrWhiteSpace(levelObjective);
        GameObject panel = CreatePanel("HeaderPanel", parent, panelColor);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(22f, -22f);
        rect.sizeDelta = hasObjective ? new Vector2(620f, 140f) : new Vector2(620f, 76f);

        titleText = CreateText("Title", panel.transform, levelTitle, 48, FontStyle.Bold, accentColor);
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = hasObjective ? new Vector2(0f, 1f) : Vector2.zero;
        titleRect.anchorMax = hasObjective ? new Vector2(1f, 1f) : Vector2.one;
        titleRect.pivot = hasObjective ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0.5f);
        titleRect.offsetMin = hasObjective ? new Vector2(20f, -48f) : new Vector2(20f, 0f);
        titleRect.offsetMax = hasObjective ? new Vector2(-20f, -8f) : new Vector2(-20f, 0f);
        titleText.alignment = hasObjective ? TextAnchor.UpperLeft : TextAnchor.MiddleLeft;

        if (!hasObjective)
        {
            return;
        }

        objectiveText = CreateText("Subtitle", panel.transform, levelObjective, 40, FontStyle.Normal, Color.white);
        RectTransform subtitleRect = objectiveText.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 0f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.offsetMin = new Vector2(20f, 16f);
        subtitleRect.offsetMax = new Vector2(-20f, -48f);
    }

    private void CreateMenuButton(Transform parent)
    {
        GameObject buttonObject = new("MenuButton");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(ToggleMenu);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-22f, -22f);
        rect.sizeDelta = new Vector2(190f, 60f);

        Text label = CreateText("Label", buttonObject.transform, "Menu", 26, FontStyle.Bold, buttonTextColor);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAnchor.MiddleCenter;
    }

    private GameObject CreateMenuOverlay(Transform parent)
    {
        GameObject overlay = CreatePanel("MenuOverlay", parent, overlayColor);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        GameObject panel = CreatePanel("MenuPanel", overlay.transform, panelColor);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(460f, 360f);

        Text title = CreateText("MenuTitle", panel.transform, "Menu", 52, FontStyle.Bold, accentColor);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(40f, -88f);
        titleRect.offsetMax = new Vector2(-40f, -24f);
        title.alignment = TextAnchor.MiddleCenter;

        CreateOverlayButton(panel.transform, "ResumeButton", "Continue", new Vector2(0f, -28f), HideMenu);
        CreateOverlayButton(panel.transform, "RestartButton", "Restart", new Vector2(0f, -102f), RestartLevel);
        CreateOverlayButton(panel.transform, "LevelMenuButton", "Level Menu", new Vector2(0f, -176f), LoadMenu);

        return overlay;
    }

    private GameObject CreateFailureOverlay(Transform parent)
    {
        GameObject overlay = CreatePanel("FailureOverlay", parent, overlayColor);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        GameObject panel = CreatePanel("FailurePanel", overlay.transform, panelColor);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(620f, 520f);

        failureTitleText = CreateText("FailureTitle", panel.transform, GetSoftFailureTitle(failureTitle), 52, FontStyle.Bold, dangerColor);
        RectTransform titleRect = failureTitleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(40f, -92f);
        titleRect.offsetMax = new Vector2(-40f, -28f);
        failureTitleText.alignment = TextAnchor.MiddleCenter;

        failureMessageText = CreateText("FailureMessage", panel.transform, failureMessage, 30, FontStyle.Normal, Color.white);
        RectTransform messageRect = failureMessageText.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0f, 1f);
        messageRect.anchorMax = new Vector2(1f, 1f);
        messageRect.pivot = new Vector2(0.5f, 1f);
        messageRect.offsetMin = new Vector2(54f, -160f);
        messageRect.offsetMax = new Vector2(-54f, -98f);
        failureMessageText.alignment = TextAnchor.MiddleCenter;

        CreateOverlayButton(panel.transform, "RestartButton", "Restart", new Vector2(0f, -82f), RestartFailureAttempt);
        failureCheckpointButton = CreateOverlayButton(panel.transform, "CheckpointButton", "Restart from checkpoint", new Vector2(0f, -156f), RestartFromLastCheckpoint).transform.parent.gameObject;
        failureMenuButton = CreateOverlayButton(panel.transform, "MenuButton", "Menu", new Vector2(0f, -230f), LoadMenu).transform.parent.gameObject;
        RefreshFailureButtons();

        return overlay;
    }

    private Text CreateOverlayButton(Transform parent, string name, string labelText, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new(name);
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(390f, 58f);

        Text label = CreateText("Label", buttonObject.transform, labelText, 28, FontStyle.Bold, buttonTextColor);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAnchor.MiddleCenter;
        return label;
    }

    private void CreateControlsHint(Transform parent)
    {
        if (string.IsNullOrWhiteSpace(controlsHint))
        {
            return;
        }

        GameObject panel = CreatePanel("ControlsHint", parent, panelColor);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 22f);
        rect.sizeDelta = new Vector2(500f, 60f);

        controlsHintText = CreateText("HintText", panel.transform, controlsHint, 24, FontStyle.Bold, Color.white);
        RectTransform textRect = controlsHintText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        controlsHintText.alignment = TextAnchor.MiddleCenter;
    }

    private Text CreateCompletionLabel(Transform parent)
    {
        Text text = CreateText("CompletionText", parent, completionMessage, 48, FontStyle.Bold, accentColor);
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

    private void RefreshUiText()
    {
        if (titleText != null)
        {
            titleText.text = levelTitle;
        }

        if (objectiveText != null)
        {
            objectiveText.text = levelObjective;
        }

        if (controlsHintText != null)
        {
            controlsHintText.text = controlsHint;
        }

        if (completionText != null)
        {
            completionText.text = completionMessage;
        }
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

    private void RefreshFailureButtons()
    {
        if (failureCheckpointButton == null)
        {
            return;
        }

        bool hasCheckpoint = CheckpointManager.HasCheckpointForCurrentScene();
        failureCheckpointButton.SetActive(hasCheckpoint);

        if (failureMenuButton != null)
        {
            RectTransform menuRect = failureMenuButton.GetComponent<RectTransform>();
            menuRect.anchoredPosition = hasCheckpoint ? new Vector2(0f, -230f) : new Vector2(0f, -156f);
        }
    }

    private static string GetSoftFailureTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Take a breath";
        }

        string trimmed = title.Trim();
        return trimmed.Equals("Game over", System.StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("Gameover", System.StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("Level Failed", System.StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("Dead", System.StringComparison.OrdinalIgnoreCase)
            ? "Take a breath"
            : title;
    }
}
