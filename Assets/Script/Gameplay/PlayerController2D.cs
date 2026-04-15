using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 9f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float coyoteTime = 0.12f;

    private Rigidbody2D rb;
    private Collider2D[] ownColliders;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private bool jumpQueued;
    private float coyoteTimer;
    private bool jumpConsumed;
    private bool wasGroundedLastFrame;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ownColliders = GetComponents<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        IsGrounded = CheckGrounded();

        if (IsGrounded && !wasGroundedLastFrame)
        {
            jumpConsumed = false;
        }

        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimer > 0f && !jumpConsumed)
        {
            jumpQueued = true;
        }

        if (horizontalInput > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        wasGroundedLastFrame = IsGrounded;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (!jumpQueued)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpQueued = false;
        coyoteTimer = 0f;
        jumpConsumed = true;
    }

    private bool CheckGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.isTrigger)
            {
                continue;
            }

            bool isOwnCollider = false;

            foreach (Collider2D ownCollider in ownColliders)
            {
                if (hit == ownCollider)
                {
                    isOwnCollider = true;
                    break;
                }
            }

            if (isOwnCollider)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
