using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class ShakeOnPlayerTouch : MonoBehaviour
{
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 0.2f;
    public string playerTag = "Player";


    public bool triggerOnce = true;
    private bool hasTriggered = false;
    private Camera targetCamera;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        targetCamera = Camera.main;

        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered && targetCamera != null)
        {
            TriggerScreenShake();
            
            if (triggerOnce)
            {
                hasTriggered = true;
            }
        }
    }

    private void TriggerScreenShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        Transform cameraTransform = targetCamera.transform;
        Vector3 originalPosition = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float strength = Mathf.Lerp(shakeIntensity, 0f, elapsed / shakeDuration);
            Vector2 offset = Random.insideUnitCircle * strength;
            cameraTransform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
        shakeRoutine = null;
    }

    public void ManualTriggerShake()
    {
        if (targetCamera != null && (!triggerOnce || !hasTriggered))
        {
            TriggerScreenShake();
            if (triggerOnce) hasTriggered = true;
        }
    }

    private void OnDestroy()
    {
        if (targetCamera != null && shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
    }
    
}
