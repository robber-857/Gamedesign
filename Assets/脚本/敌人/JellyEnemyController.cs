using UnityEngine;

public class JellyEnemyController : MonoBehaviour
{
    [Header("Player Bounce")]
    public string playerTag = "Player";
    public float bounceVelocity = 10f;
    public bool requirePlayerAbove = true;
    public float stompTopTolerance = 0.15f;
    public bool destroyAfterBounce = true;
    public float destroyDelay = 0.05f;

    public Rigidbody2D _rigidbody2D;
    private Collider2D jellyCollider;
    private bool hasBounced;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        jellyCollider = GetComponent<Collider2D>();

        if (jellyCollider == null)
        {
            jellyCollider = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        TryBouncePlayer(other.collider, other.rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryBouncePlayer(other, other.attachedRigidbody);
    }

    private void TryBouncePlayer(Collider2D playerCollider, Rigidbody2D playerBody)
    {
        if (hasBounced || playerCollider == null || !playerCollider.CompareTag(playerTag))
        {
            return;
        }

        if (playerBody == null)
        {
            playerBody = playerCollider.GetComponentInParent<Rigidbody2D>();
        }

        if (playerBody == null || !IsValidBounce(playerCollider, playerBody))
        {
            return;
        }

        PlayerController2D controller2D = playerCollider.GetComponentInParent<PlayerController2D>();

        if (controller2D != null)
        {
            controller2D.Bounce(bounceVelocity);
        }
        else
        {
            PlayerController oldController = playerCollider.GetComponentInParent<PlayerController>();

            if (oldController != null)
            {
                oldController.Jump();
            }
            else
            {
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, bounceVelocity);
            }
        }

        hasBounced = true;

        if (destroyAfterBounce)
        {
            Invoke(nameof(DestoryItem), destroyDelay);
        }
    }

    private bool IsValidBounce(Collider2D playerCollider, Rigidbody2D playerBody)
    {
        if (!requirePlayerAbove || jellyCollider == null)
        {
            return true;
        }

        bool playerFeetOnJellyTop = playerCollider.bounds.min.y >= jellyCollider.bounds.max.y - stompTopTolerance;
        bool playerMovingDownOrResting = playerBody.linearVelocity.y <= 0.5f;
        return playerFeetOnJellyTop && playerMovingDownOrResting;
    }

    public void Die()
    {
        if (_rigidbody2D != null)
        {
            _rigidbody2D.gravityScale = 1f;
        }

        if (jellyCollider != null)
        {
            jellyCollider.enabled = false;
        }

        DestoryItem();
    }

    public void DestoryItem()
    {
        Destroy(gameObject);
    }
}
