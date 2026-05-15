using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoverSceneController : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Level_0";
    [SerializeField] private string coverResourcePath = "Cover/cover";

    [Header("Colors")]
    [SerializeField] private Color fallbackBackgroundColor = new(0.04f, 0.08f, 0.05f, 1f);
    [SerializeField] private Color buttonColor = new(0.11f, 0.16f, 0.09f, 0.88f);
    [SerializeField] private Color buttonTextColor = new(1f, 0.88f, 0.36f, 1f);

    private Font cachedFont;

    private void Awake()
    {
        Time.timeScale = 1f;
        EnsureCamera();
        EnsureEventSystem();
        BuildUi();
    }

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(startSceneName))
        {
            Debug.LogWarning("Start scene name is empty.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    private void BuildUi()
    {
        Canvas canvas = CreateCanvas();
        CreateBackground(canvas.transform);
        CreateStartButton(canvas.transform);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new("CoverCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private void CreateBackground(Transform parent)
    {
        GameObject backgroundObject = new("CoverBackground");
        backgroundObject.transform.SetParent(parent, false);

        RawImage image = backgroundObject.AddComponent<RawImage>();
        Texture2D coverTexture = Resources.Load<Texture2D>(coverResourcePath);

        if (coverTexture != null)
        {
            image.texture = coverTexture;
            image.color = Color.white;
        }
        else
        {
            image.color = fallbackBackgroundColor;
        }

        RectTransform rect = backgroundObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = coverTexture != null ? (float)coverTexture.width / coverTexture.height : 16f / 9f;
    }

    private void CreateStartButton(Transform parent)
    {
        GameObject buttonObject = new("StartButton");
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(StartGame);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 92f);
        rect.sizeDelta = new Vector2(300f, 76f);

        Text label = CreateText("Label", buttonObject.transform, "Start", 34, FontStyle.Bold, buttonTextColor);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.alignment = TextAnchor.MiddleCenter;
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
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            sceneCamera = cameraObject.AddComponent<Camera>();

            if (FindFirstObjectByType<AudioListener>() == null)
            {
                cameraObject.AddComponent<AudioListener>();
            }
        }

        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = fallbackBackgroundColor;
        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 5f;
        sceneCamera.cullingMask = 0;
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
