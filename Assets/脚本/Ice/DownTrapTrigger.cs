using UnityEngine;
using UnityEngine.SceneManagement;

public class DownTrapTrigger : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";
    public bool requirePlayerAbove = true;
    public float topCheckTolerance = 0.25f;
    public bool triggerOnlyOnce = true;

    [Header("Trap")]
    public Rigidbody2D downTrap;
    public GameObject downTrapPrefab;
    public Transform spawnPoint;
    public string downTrapName = "Down1";
    public bool autoFindNearestTrap = true;
    public string autoFindTrapNamePrefix = "Down";
    public float fallGravityScale = 4f;
    public float initialFallSpeed = 0f;
    public bool freezeTrapRotation = true;

    [Header("Pass Through Ice")]
    public bool passThroughIceGround = true;
    public bool makeTrapTriggerWhenFalling = true;
    public Transform iceGroundRoot;
    public string iceGroundRootName = "IceGroundlayer";

    private Collider2D triggerCollider;
    private bool hasTriggered;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        FindTrapIfNeeded();
        PrepareTrap();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryTrigger(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryTrigger(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTrigger(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryTrigger(other);
    }

    private void TryTrigger(Collider2D other)
    {
        if (hasTriggered && triggerOnlyOnce)
        {
            return;
        }

        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        if (requirePlayerAbove && !IsPlayerAbove(other))
        {
            return;
        }

        FindTrapIfNeeded();

        if (downTrap == null)
        {
            CreateTrapFromPrefab();
        }

        if (downTrap == null)
        {
            Debug.LogWarning("DownTrapTrigger could not find or create Down1. Assign Down Trap or Down Trap Prefab in the inspector.", this);
            return;
        }

        hasTriggered = true;
        StartTrapFall();
    }

    private bool IsPlayerAbove(Collider2D playerCollider)
    {
        if (triggerCollider == null)
        {
            return true;
        }

        return playerCollider.bounds.min.y >= triggerCollider.bounds.max.y - topCheckTolerance;
    }

    private void FindTrapIfNeeded()
    {
        if (downTrap != null)
        {
            return;
        }

        if (autoFindNearestTrap)
        {
            downTrap = FindNearestTrap();

            if (downTrap != null)
            {
                return;
            }
        }

        GameObject trapObject = GameObject.Find(downTrapName);

        if (trapObject != null)
        {
            downTrap = trapObject.GetComponent<Rigidbody2D>();
        }
    }

    private Rigidbody2D FindNearestTrap()
    {
        Rigidbody2D[] bodies = FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Rigidbody2D nearestTrap = null;
        float nearestDistance = float.MaxValue;

        foreach (Rigidbody2D body in bodies)
        {
            if (body == null || body.transform == transform)
            {
                continue;
            }

            string objectName = body.gameObject.name;
            bool matchesPrefix = !string.IsNullOrEmpty(autoFindTrapNamePrefix)
                && objectName.StartsWith(autoFindTrapNamePrefix, System.StringComparison.OrdinalIgnoreCase);
            bool matchesExactName = !string.IsNullOrEmpty(downTrapName)
                && objectName.Equals(downTrapName, System.StringComparison.OrdinalIgnoreCase);

            if (!matchesPrefix && !matchesExactName)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, body.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTrap = body;
            }
        }

        return nearestTrap;
    }

    private void PrepareTrap()
    {
        if (downTrap == null)
        {
            return;
        }

        downTrap.gravityScale = 0f;
        downTrap.linearVelocity = Vector2.zero;
        downTrap.angularVelocity = 0f;

        if (freezeTrapRotation)
        {
            downTrap.constraints |= RigidbodyConstraints2D.FreezeRotation;
        }

        DownTrapHitDetector hitDetector = downTrap.GetComponent<DownTrapHitDetector>();

        if (hitDetector == null)
        {
            hitDetector = downTrap.gameObject.AddComponent<DownTrapHitDetector>();
        }

        hitDetector.playerTag = playerTag;
    }

    private void CreateTrapFromPrefab()
    {
        if (downTrapPrefab == null)
        {
            return;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : downTrapPrefab.transform.position;
        Quaternion rotation = downTrapPrefab.transform.rotation;
        GameObject trapObject = Instantiate(downTrapPrefab, position, rotation);
        trapObject.name = downTrapName;
        downTrap = trapObject.GetComponent<Rigidbody2D>();
        PrepareTrap();
    }

    private void StartTrapFall()
    {
        if (passThroughIceGround)
        {
            IgnoreIceGroundCollisions();
        }

        if (makeTrapTriggerWhenFalling)
        {
            SetTrapCollidersTrigger(true);
        }

        downTrap.bodyType = RigidbodyType2D.Dynamic;
        downTrap.simulated = true;
        downTrap.gravityScale = fallGravityScale;
        downTrap.linearVelocity = new Vector2(downTrap.linearVelocity.x, -Mathf.Abs(initialFallSpeed));
        downTrap.WakeUp();
    }

    private void IgnoreIceGroundCollisions()
    {
        if (downTrap == null)
        {
            return;
        }

        if (iceGroundRoot == null)
        {
            GameObject iceGroundObject = GameObject.Find(iceGroundRootName);

            if (iceGroundObject != null)
            {
                iceGroundRoot = iceGroundObject.transform;
            }
        }

        if (iceGroundRoot == null)
        {
            return;
        }

        Collider2D[] trapColliders = downTrap.GetComponentsInChildren<Collider2D>();
        Collider2D[] iceColliders = iceGroundRoot.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D trapCollider in trapColliders)
        {
            if (trapCollider == null)
            {
                continue;
            }

            foreach (Collider2D iceCollider in iceColliders)
            {
                if (iceCollider != null)
                {
                    Physics2D.IgnoreCollision(trapCollider, iceCollider, true);
                }
            }
        }
    }

    private void SetTrapCollidersTrigger(bool isTrigger)
    {
        if (downTrap == null)
        {
            return;
        }

        Collider2D[] trapColliders = downTrap.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D trapCollider in trapColliders)
        {
            if (trapCollider != null)
            {
                trapCollider.isTrigger = isTrigger;
            }
        }
    }
}

public class DownTrapHitDetector : MonoBehaviour
{
    public string playerTag = "Player";

    private bool hasFailed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryFail(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryFail(other);
    }

    private void TryFail(Collider2D other)
    {
        if (hasFailed || other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        hasFailed = true;
        LevelUIController ui = LevelUIController.FindOrCreate();

        if (ui != null)
        {
            ui.ShowFailure();
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
