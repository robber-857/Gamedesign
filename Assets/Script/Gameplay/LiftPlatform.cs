using UnityEngine;

public class LiftPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 targetOffset = Vector3.up * 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool activateOnlyOnce = true;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActivated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        targetPosition = startPosition + targetOffset;
    }

    private void FixedUpdate()
    {
        if (!isActivated)
        {
            return;
        }

        Vector3 nextPosition = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime);

        if (rb != null)
        {
            rb.MovePosition(nextPosition);
            return;
        }

        transform.position = nextPosition;
    }

    public void Activate()
    {
        if (activateOnlyOnce && isActivated)
        {
            return;
        }

        isActivated = true;
    }
}
