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

    private Collider2D[] iceColliders;
    private Collider2D playerCollider;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private PlayerController2D playerController2D;

    private void Awake()
    {
        iceColliders = GetComponentsInChildren<Collider2D>();
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
            iceColliders = GetComponentsInChildren<Collider2D>();
        }

        if (playerRb == null || playerCollider == null)
        {
            return;
        }

        bool playerOnIce = IsPlayerStandingOnIce();

        if (playerController != null)
        {
            playerController.canMove = !playerOnIce;
        }

        if (playerController2D != null)
        {
            playerController2D.CanMove = !playerOnIce;
        }

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
        if (playerController != null)
        {
            playerController.canMove = true;
        }

        if (playerController2D != null)
        {
            playerController2D.CanMove = true;
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
            foreach (Collider2D iceCollider in iceColliders)
            {
                if (hit == iceCollider)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
