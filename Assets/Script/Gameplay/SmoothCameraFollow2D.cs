using UnityEngine;

public class SmoothCameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private Vector2 followOffset = new(1.6f, 0.8f);
    [SerializeField] private Vector2 deadZone = new(1.4f, 0.9f);
    [SerializeField] private float smoothTime = 0.45f;
    [SerializeField] private bool followY = true;

    private Vector3 velocity;

    private void Awake()
    {
        FindTargetIfNeeded();
    }

    private void LateUpdate()
    {
        FindTargetIfNeeded();

        if (target == null)
        {
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 desiredPosition = currentPosition;
        Vector3 targetPosition = target.position + new Vector3(followOffset.x, followOffset.y, 0f);
        Vector2 distance = targetPosition - currentPosition;

        if (Mathf.Abs(distance.x) > deadZone.x)
        {
            desiredPosition.x = targetPosition.x - (Mathf.Sign(distance.x) * deadZone.x);
        }

        if (followY && Mathf.Abs(distance.y) > deadZone.y)
        {
            desiredPosition.y = targetPosition.y - (Mathf.Sign(distance.y) * deadZone.y);
        }

        desiredPosition.z = currentPosition.z;
        transform.position = Vector3.SmoothDamp(currentPosition, desiredPosition, ref velocity, smoothTime);
    }

    private void FindTargetIfNeeded()
    {
        if (target != null || string.IsNullOrWhiteSpace(targetTag))
        {
            return;
        }

        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);

        if (targetObject != null)
        {
            target = targetObject.transform;
        }
    }
}
