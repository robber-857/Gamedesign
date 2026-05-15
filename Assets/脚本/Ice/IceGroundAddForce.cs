using UnityEngine;

[DefaultExecutionOrder(10000)]
public class IceGroundAddForce : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Ice Movement")]
    public float acceleration = 18f;
    public float maxSlideSpeed = 12f;
    public float minSlideSpeed = 2.5f;

    [Header("Ground Check")]
    public float footTolerance = 0.12f;
    public float horizontalPadding = 0.05f;
    public bool includeTaggedIceGround = true;

    private Collider2D[] iceColliders;
    private Collider2D playerCollider;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private PlayerController2D playerController2D;

    private void Awake()
    {
        RefreshIceColliders();
        FindPlayer();
    }

    private void FixedUpdate()
    {
        if (player == null || playerRb == null || playerCollider == null)
        {
            FindPlayer();
        }

        if (iceColliders == null || iceColliders.Length == 0)
        {
            RefreshIceColliders();
        }

        if (playerRb == null || playerCollider == null)
        {
            return;
        }

        if (transform == player)
        {
            SetPlayerMovementEnabled(true);
            return;
        }

        bool playerOnIce = IsPlayerStandingOnIce();

        SetPlayerMovementEnabled(!playerOnIce);

        if (!playerOnIce)
        {
            return;
        }

        float velocityX = playerRb.linearVelocity.x + acceleration * Time.fixedDeltaTime;

        if (velocityX < minSlideSpeed)
        {
            velocityX = minSlideSpeed;
        }

        velocityX = Mathf.Clamp(velocityX, minSlideSpeed, maxSlideSpeed);
        playerRb.linearVelocity = new Vector2(velocityX, playerRb.linearVelocity.y);
    }

    private void OnDisable()
    {
        SetPlayerMovementEnabled(true);
    }

    private void SetPlayerMovementEnabled(bool enabled)
    {
        if (playerController != null)
        {
            playerController.canMove = enabled;
        }

        if (playerController2D != null)
        {
            playerController2D.CanMove = enabled;
        }
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            return;
        }

        playerRb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();
        playerController = player.GetComponent<PlayerController>();
        playerController2D = player.GetComponent<PlayerController2D>();
    }

    private void RefreshIceColliders()
    {
        iceColliders = GetComponentsInChildren<Collider2D>();

        if (!includeTaggedIceGround || iceColliders.Length > 0)
        {
            return;
        }

        GameObject[] iceObjects = GameObject.FindGameObjectsWithTag("IceGround");
        var colliders = new System.Collections.Generic.List<Collider2D>();

        foreach (GameObject iceObject in iceObjects)
        {
            if (iceObject == null || iceObject.transform == transform)
            {
                continue;
            }

            colliders.AddRange(iceObject.GetComponentsInChildren<Collider2D>());
        }

        iceColliders = colliders.ToArray();
    }

    private bool IsPlayerStandingOnIce()
    {
        Bounds playerBounds = playerCollider.bounds;
        Vector2 checkCenter = new Vector2(playerBounds.center.x, playerBounds.min.y - footTolerance * 0.5f);
        Vector2 checkSize = new Vector2(
            Mathf.Max(0.05f, playerBounds.size.x - horizontalPadding * 2f),
            Mathf.Max(0.02f, footTolerance));

        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, checkSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit == playerCollider)
            {
                continue;
            }

            foreach (Collider2D iceCollider in iceColliders)
            {
                if (iceCollider != null && iceCollider != playerCollider && hit == iceCollider)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
