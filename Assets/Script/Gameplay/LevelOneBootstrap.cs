using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LevelOneBootstrap : MonoBehaviour
{
    private static Sprite sharedSprite;
    private const string FloorBlockPrefabPath = "Assets/Prefab/Floor_Block.prefab";
    private const string PlatformStonePrefabPath = "Assets/Prefab/Platformstone.prefab";
    private const string LiftBoxPrefabPath = "Assets/Prefab/lift_box.prefab";
    private const string ExitDoorPrefabPath = "Assets/Prefab/Exit_door.prefab";
    private const string PlayerSpritePath = "Assets/Art/Characters/MuseumExplorer/museum_explorer_idle.png";
    private const string ButtonUpSpritePath = "Assets/Art/Props/Buttons/button_b1.png";
    private const string ButtonDownSpritePath = "Assets/Art/Props/Buttons/button_b2.png";

    private readonly Color backdropColor = new(0.08f, 0.12f, 0.2f, 1f);
    private readonly Color hallColor = new(0.16f, 0.2f, 0.29f, 1f);
    private readonly Color trimColor = new(0.75f, 0.67f, 0.48f, 1f);
    private readonly Color platformColor = new(0.46f, 0.5f, 0.58f, 1f);
    private readonly Color liftColor = new(0.52f, 0.7f, 0.78f, 1f);
    private readonly Color buttonColor = new(0.83f, 0.56f, 0.28f, 1f);
    private readonly Color exitColor = new(0.66f, 0.86f, 0.72f, 1f);
    private readonly Color playerColor = new(0.96f, 0.92f, 0.78f, 1f);
    private readonly Color hazardColor = new(0.72f, 0.24f, 0.22f, 1f);

    [Header("Layout")]
    [SerializeField] private Vector2 playerSpawnPosition = new(-9.9f, -3.68f);
    [SerializeField] private Vector2 entryLiftStartPosition = new(-3.6f, -4.05f);
    [SerializeField] private Vector2 archiveLiftStartPosition = new(7.45f, -1.55f);
    [SerializeField] private float playerGroundSnapOffset = 0.01f;

    private Transform decoRoot;
    private Transform groundRoot;
    private Transform platformRoot;
    private Transform triggerRoot;
    private Transform hazardRoot;
    private BoxCollider2D startFloorCollider;
    private bool isGenerating;

    private void OnEnable()
    {
        if (!CanGeneratePreview() || HasGeneratedChildren())
        {
            return;
        }

        LoadEditorDefaults();
        GenerateLevel();
    }

    [ContextMenu("Rebuild Level Preview")]
    private void RebuildLevelPreview()
    {
        if (!CanGeneratePreview())
        {
            return;
        }

        LoadEditorDefaults();
        GenerateLevel();
    }

    [ContextMenu("Clear Level Preview")]
    private void ClearLevelPreview()
    {
        if (Application.isPlaying || isGenerating)
        {
            return;
        }

        ClearGeneratedChildren();
    }

    private bool CanGeneratePreview()
    {
#if UNITY_EDITOR
        return !Application.isPlaying
            && !EditorApplication.isPlayingOrWillChangePlaymode
            && isActiveAndEnabled
            && gameObject.scene.IsValid()
            && SceneManager.GetActiveScene() == gameObject.scene
            && !isGenerating;
#else
        return !Application.isPlaying
            && isActiveAndEnabled
            && gameObject.scene.IsValid()
            && SceneManager.GetActiveScene() == gameObject.scene
            && !isGenerating;
#endif
    }

    private bool HasGeneratedChildren()
    {
        return transform.childCount > 0;
    }

    private void GenerateLevel()
    {
        isGenerating = true;

        try
        {
            ClearGeneratedChildren();
            ConfigureCamera();
            EnsureGlobalLight();
            CreateRoots();
            BuildBackdrop();
            BuildCourse();
            SpawnPlayer();
            ConfigureUi();
        }
        finally
        {
            isGenerating = false;
        }
    }

    private void ClearGeneratedChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }

    private void ConfigureCamera()
    {
        Camera sceneCamera = Camera.main;

        if (sceneCamera == null)
        {
            sceneCamera = FindFirstObjectByType<Camera>();
        }

        if (sceneCamera == null)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            sceneCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 6.5f;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = backdropColor;
        sceneCamera.transform.position = new Vector3(0f, -0.5f, -10f);

        if (sceneCamera.GetComponent<UniversalAdditionalCameraData>() == null)
        {
            sceneCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
        }
    }

    private void LoadEditorDefaults()
    {
#if UNITY_EDITOR
        // Force assets to import before preview generation if the editor has just refreshed.
        AssetDatabase.LoadAssetAtPath<GameObject>(FloorBlockPrefabPath);
#endif
    }

    private void EnsureGlobalLight()
    {
        if (FindFirstObjectByType<Light2D>() != null)
        {
            return;
        }

        GameObject lightObject = new("Global Light 2D");
        lightObject.transform.SetParent(transform, false);

        Light2D light2D = lightObject.AddComponent<Light2D>();
        light2D.lightType = Light2D.LightType.Global;
        light2D.intensity = 1f;
        light2D.color = Color.white;
    }

    private void CreateRoots()
    {
        decoRoot = CreateRoot("DecoRoot");
        groundRoot = CreateRoot("GroundRoot");
        platformRoot = CreateRoot("PlatformRoot");
        triggerRoot = CreateRoot("TriggerRoot");
        hazardRoot = CreateRoot("HazardRoot");
    }

    private void BuildBackdrop()
    {
        CreateBlock("SkyBackdrop", new Vector2(0f, -0.5f), new Vector2(23f, 13.5f), backdropColor, decoRoot, false, -10);
        CreateBlock("HallBackdrop", new Vector2(0f, -0.3f), new Vector2(20.5f, 9.75f), hallColor, decoRoot, false, -9);
        CreateBlock("Cornice", new Vector2(0f, 3.95f), new Vector2(20.5f, 0.32f), trimColor, decoRoot, false, -8);
        CreateBlock("FloorTrim", new Vector2(0f, -4.05f), new Vector2(20.5f, 0.3f), trimColor, decoRoot, false, -8);

        CreateBlock("ColumnLeft", new Vector2(-5.4f, -0.45f), new Vector2(0.45f, 7.5f), trimColor, decoRoot, false, -7);
        CreateBlock("ColumnMid", new Vector2(0.2f, -0.45f), new Vector2(0.45f, 7.5f), trimColor, decoRoot, false, -7);
        CreateBlock("ColumnRight", new Vector2(5.8f, -0.45f), new Vector2(0.45f, 7.5f), trimColor, decoRoot, false, -7);

        CreateBlock("FrameLeft", new Vector2(-8.2f, 1.65f), new Vector2(1.7f, 2.3f), trimColor, decoRoot, false, -6);
        CreateBlock("FrameCenter", new Vector2(-2.7f, 1.65f), new Vector2(1.7f, 2.3f), trimColor, decoRoot, false, -6);
        CreateBlock("FrameRight", new Vector2(8.7f, 1.65f), new Vector2(1.7f, 2.3f), trimColor, decoRoot, false, -6);

        CreateBlock("PaintingLeft", new Vector2(-8.2f, 1.65f), new Vector2(1.3f, 1.9f), new Color(0.32f, 0.44f, 0.57f, 1f), decoRoot, false, -5);
        CreateBlock("PaintingCenter", new Vector2(-2.7f, 1.65f), new Vector2(1.3f, 1.9f), new Color(0.49f, 0.38f, 0.3f, 1f), decoRoot, false, -5);
        CreateBlock("PaintingRight", new Vector2(8.7f, 1.65f), new Vector2(1.3f, 1.9f), new Color(0.28f, 0.46f, 0.39f, 1f), decoRoot, false, -5);
    }

    private void BuildCourse()
    {
        startFloorCollider = null;

        CreateBlock("LeftBoundary", new Vector2(-11.3f, -0.3f), new Vector2(0.8f, 14f), hallColor, groundRoot, true, -1);
        CreateBlock("RightBoundary", new Vector2(11.3f, -0.3f), new Vector2(0.8f, 14f), hallColor, groundRoot, true, -1);

        GameObject startFloor = CreateFloorPiece("StartFloor", new Vector2(-8.1f, -4.8f), new Vector2(5.4f, 1.35f));
        startFloorCollider = startFloor != null ? startFloor.GetComponent<BoxCollider2D>() : null;
        CreateFloorPiece("MiddleFloor", new Vector2(-0.9f, -4.8f), new Vector2(2.7f, 1.35f));
        CreateStonePlatform("UpperWalkway", new Vector2(1.05f, -0.25f), new Vector2(6.1f, 0.7f));
        CreateStonePlatform("SwitchPedestal", new Vector2(4.8f, 0.7f), new Vector2(1.7f, 0.8f));
        CreateStonePlatform("ExitBalcony", new Vector2(9.55f, 1.95f), new Vector2(3.6f, 0.7f));

        LiftPlatform archiveLift = CreateLift("ArchiveLift", archiveLiftStartPosition, new Vector2(2f, 0.45f), new Vector2(0f, 3.5f));
        LiftPlatform entryLift = CreateLift("EntryLift", entryLiftStartPosition, new Vector2(1.9f, 0.45f), new Vector2(0f, 3.25f));

        CreateButton("EntrySwitch", new Vector2(-6.55f, -3.95f), entryLift);
        CreateButton("ArchiveSwitch", new Vector2(4.8f, 1.25f), archiveLift);
        CreateExitDoor(new Vector2(10.35f, 2.95f));
        CreateHazardZone("ResetZone", new Vector2(0f, -7.35f), new Vector2(25f, 1.3f));

        CreateBlock("LiftTrimLeft", new Vector2(-3.6f, -0.1f), new Vector2(0.2f, 6.8f), trimColor, decoRoot, false, -4);
        CreateBlock("LiftTrimRight", new Vector2(7.45f, 0.95f), new Vector2(0.2f, 4.8f), trimColor, decoRoot, false, -4);
        CreateBlock("ExitArch", new Vector2(10.35f, 2.7f), new Vector2(1.7f, 2.6f), trimColor, decoRoot, false, -3);
        CreateBlock("ExitDoorFill", new Vector2(10.35f, 2.55f), new Vector2(1.15f, 2f), new Color(0.27f, 0.44f, 0.37f, 1f), decoRoot, false, -2);
        CreateBlock("HazardGlow", new Vector2(0f, -6.9f), new Vector2(25f, 0.22f), hazardColor, decoRoot, false, -2);
    }

    private void SpawnPlayer()
    {
        GameObject player = CreateBlock("MuseumExplorer", playerSpawnPosition, new Vector2(1f, 1f), playerColor, transform, false, 6);
        player.tag = "Player";
        player.transform.localScale = Vector3.one;

        SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();
        Sprite playerSprite = LoadSprite(PlayerSpritePath);

        if (renderer != null && playerSprite != null)
        {
            renderer.sprite = playerSprite;
            renderer.color = Color.white;
        }

        CapsuleCollider2D collider = player.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.56f, 0.86f);
        collider.offset = new Vector2(0f, -0.01f);
        collider.direction = CapsuleDirection2D.Vertical;

        SnapPlayerToStartFloor(player.transform, collider.bounds.min.y);

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform, false);
        groundCheck.localPosition = new Vector3(0f, -0.48f, 0f);

        PlayerController2D controller = player.AddComponent<PlayerController2D>();
        controller.Configure(groundCheck, 4.25f, 7.6f, 0.12f, 0.15f, 0.92f);
    }

    private void SnapPlayerToStartFloor(Transform playerTransform, float playerFeetY)
    {
        if (startFloorCollider == null)
        {
            return;
        }

        Bounds floorBounds = startFloorCollider.bounds;
        float floorTopY = floorBounds.max.y;
        float correction = (floorTopY - playerFeetY) + playerGroundSnapOffset;

        Vector3 position = playerTransform.position;
        position.y += correction;
        playerTransform.position = position;
    }

    private void ConfigureUi()
    {
        LevelUIController ui = FindFirstObjectByType<LevelUIController>();

        if (ui == null)
        {
            GameObject uiRoot = new("UIROOT");
            uiRoot.transform.SetParent(transform, false);
            ui = uiRoot.AddComponent<LevelUIController>();
        }

        ui.Configure(
            "Level 1 Yixue Shi",
            "Wake both exhibit lifts and reach the archive door.",
            "WASD Move   Space Jump",
            "Archive unlocked!");
    }

    private Transform CreateRoot(string name)
    {
        Transform existing = transform.Find(name);

        if (existing != null)
        {
            return existing;
        }

        GameObject root = new(name);
        root.transform.SetParent(transform, false);
        return root.transform;
    }

    private GameObject CreateStaticPlatform(string name, Vector2 position, Vector2 size)
    {
        return CreateBlock(name, position, size, platformColor, groundRoot, true, 0);
    }

    private GameObject CreateFloorPiece(string name, Vector2 position, Vector2 size)
    {
        GameObject floor = InstantiateVisual(FloorBlockPrefabPath, name, groundRoot, position, size, platformColor);

        if (floor == null)
        {
            return CreateStaticPlatform(name, position, size);
        }

        EnsureBoxCollider(floor);
        SetSortingOrder(floor, 0);
        return floor;
    }

    private GameObject CreateStonePlatform(string name, Vector2 position, Vector2 size)
    {
        GameObject platform = InstantiateVisual(PlatformStonePrefabPath, name, groundRoot, position, size, Color.white);

        if (platform == null)
        {
            return CreateStaticPlatform(name, position, size);
        }

        EnsureBoxCollider(platform);
        SetSortingOrder(platform, 1);
        return platform;
    }

    private LiftPlatform CreateLift(string name, Vector2 position, Vector2 size, Vector2 targetOffset)
    {
        GameObject liftObject = InstantiateVisual(LiftBoxPrefabPath, name, platformRoot, position, size, Color.white);

        if (liftObject == null)
        {
            liftObject = CreateBlock(name, position, size, liftColor, platformRoot, true, 2);
        }
        else
        {
            EnsureBoxCollider(liftObject);
            SetSortingOrder(liftObject, 2);
        }

        Rigidbody2D rb = liftObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;

        LiftPlatform lift = liftObject.AddComponent<LiftPlatform>();
        lift.Configure(targetOffset, 2.35f, true);
        return lift;
    }

    private void CreateButton(string name, Vector2 position, LiftPlatform targetLift)
    {
        GameObject buttonObject = CreateBlock(name, position, new Vector2(1f, 2f), buttonColor, triggerRoot, true, 3);

        SpriteRenderer renderer = buttonObject.GetComponent<SpriteRenderer>();
        Sprite buttonSprite = LoadSprite(ButtonUpSpritePath);

        if (renderer != null && buttonSprite != null)
        {
            renderer.sprite = buttonSprite;
            renderer.color = Color.white;
        }

        BoxCollider2D collider = buttonObject.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.35f, 0.2f);
        collider.offset = new Vector2(0f, -0.06f);

        PressureButton button = buttonObject.AddComponent<PressureButton>();
        button.Configure(new[] { targetLift });
    }

    private void CreateExitDoor(Vector2 position)
    {
        GameObject exitDoor = InstantiateVisual(ExitDoorPrefabPath, "ExitDoor", triggerRoot, position, new Vector2(1.15f, 2.25f), Color.white);

        if (exitDoor == null)
        {
            exitDoor = CreateBlock("ExitDoor", position, new Vector2(0.9f, 1.65f), exitColor, triggerRoot, true, 4);
        }
        else
        {
            EnsureBoxCollider(exitDoor);
            SetSortingOrder(exitDoor, 4);
        }

        BoxCollider2D collider = exitDoor.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.52f, 0.9f);
        collider.offset = new Vector2(0f, -0.2f);

        ExitDoorTrigger trigger = exitDoor.AddComponent<ExitDoorTrigger>();
        trigger.Configure("Player", string.Empty);
    }

    private void CreateHazardZone(string name, Vector2 position, Vector2 size)
    {
        GameObject hazardZone = CreateBlock(name, position, size, hazardColor, hazardRoot, true, -1);
        hazardZone.AddComponent<HazardResetZone>();
    }

    private GameObject CreateBlock(
        string name,
        Vector2 position,
        Vector2 size,
        Color color,
        Transform parent,
        bool withCollider,
        int sortingOrder)
    {
        GameObject block = new(name);
        block.transform.SetParent(parent, false);
        block.transform.position = new Vector3(position.x, position.y, 0f);
        block.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = GetSolidSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        if (withCollider)
        {
            block.AddComponent<BoxCollider2D>();
        }

        return block;
    }

    private GameObject InstantiateVisual(
        string assetPath,
        string instanceName,
        Transform parent,
        Vector2 position,
        Vector2 size,
        Color tint)
    {
#if UNITY_EDITOR
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

        if (prefab == null)
        {
            return null;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

        if (instance == null)
        {
            return null;
        }

        instance.name = instanceName;
        instance.transform.localPosition = new Vector3(position.x, position.y, 0f);
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = tint;
        }

        return instance;
#else
        return null;
#endif
    }

    private Sprite LoadSprite(string assetPath)
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
#else
        return null;
#endif
    }

    private static void EnsureBoxCollider(GameObject target)
    {
        if (target.GetComponent<BoxCollider2D>() == null)
        {
            target.AddComponent<BoxCollider2D>();
        }
    }

    private static void SetSortingOrder(GameObject target, int sortingOrder)
    {
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
        }
    }

    private static Sprite GetSolidSprite()
    {
        if (sharedSprite != null)
        {
            return sharedSprite;
        }

        Texture2D texture = new(1, 1, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.hideFlags = HideFlags.HideAndDontSave;

        sharedSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        sharedSprite.name = "RuntimeSolidSprite";
        sharedSprite.hideFlags = HideFlags.HideAndDontSave;
        return sharedSprite;
    }
}
