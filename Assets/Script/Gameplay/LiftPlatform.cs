using UnityEngine;
using System.Collections.Generic;

public class LiftPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 targetOffset = Vector3.up * 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool activateOnlyOnce = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool carryPlayers = true;
    [SerializeField] private float topTolerance = 0.18f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActivated;
    private readonly HashSet<Rigidbody2D> riders = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
        CacheEndpoints();
    }

    private void FixedUpdate()
    {
        if (!isActivated)
        {
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime);
        Vector3 platformDelta = nextPosition - currentPosition;

        if (rb != null)
        {
            rb.MovePosition(nextPosition);
            CarryRiders(platformDelta);
            return;
        }

        transform.position = nextPosition;
        CarryRiders(platformDelta);
    }

    public void Activate()
    {
        if (activateOnlyOnce && isActivated)
        {
            return;
        }

        isActivated = true;
    }

    public void Configure(Vector3 offset, float speed = 2f, bool onlyOnce = true)
    {
        targetOffset = offset;
        moveSpeed = speed;
        activateOnlyOnce = onlyOnce;
        isActivated = false;
        CacheEndpoints();
    }

    private void CacheEndpoints()
    {
        startPosition = transform.position;
        targetPosition = startPosition + targetOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateRider(collision.collider, collision.rigidbody);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateRider(collision.collider, collision.rigidbody);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        RemoveRider(collision.rigidbody);
    }

    private void UpdateRider(Collider2D otherCollider, Rigidbody2D otherBody)
    {
        if (!carryPlayers || otherCollider == null || !otherCollider.CompareTag(playerTag))
        {
            RemoveRider(otherBody);
            return;
        }

        if (otherBody == null)
        {
            otherBody = otherCollider.GetComponentInParent<Rigidbody2D>();
        }

        if (otherBody == null)
        {
            return;
        }

        if (IsStandingOnTop(otherCollider))
        {
            riders.Add(otherBody);
        }
        else
        {
            riders.Remove(otherBody);
        }
    }

    private void RemoveRider(Rigidbody2D rider)
    {
        if (rider != null)
        {
            riders.Remove(rider);
        }
    }

    private bool IsStandingOnTop(Collider2D otherCollider)
    {
        if (platformCollider == null)
        {
            return true;
        }

        return otherCollider.bounds.min.y >= platformCollider.bounds.max.y - topTolerance;
    }

    private void CarryRiders(Vector3 platformDelta)
    {
        if (!carryPlayers || platformDelta == Vector3.zero || riders.Count == 0)
        {
            return;
        }

        foreach (Rigidbody2D rider in riders)
        {
            if (rider != null)
            {
                rider.position += (Vector2)platformDelta;
            }
        }
    }
}
