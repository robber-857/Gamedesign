using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressureButton : MonoBehaviour
{
    [SerializeField] private LiftPlatform[] targetLifts;
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private Color activatedColor = new(0.75f, 0.95f, 0.55f, 1f);

    private bool hasTriggered;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }

        var triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        if (!other.CompareTag(requiredTag))
        {
            return;
        }

        hasTriggered = true;

        foreach (var lift in targetLifts)
        {
            if (lift != null)
            {
                lift.Activate();
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = activatedColor;
        }
    }

    public void ResetVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
        }

        hasTriggered = false;
    }
}
