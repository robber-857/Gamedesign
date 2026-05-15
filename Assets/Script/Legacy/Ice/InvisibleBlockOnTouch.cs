using UnityEngine;

public class InvisibleBlockOnTouch : MonoBehaviour
{
    public string playerTag = "Player";
    public bool hideWhenPlayerLeaves = true;

    private Renderer[] renderers;
    private bool isPlayerTouching;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        SetVisible(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AddPlayerTouch(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        AddPlayerTouch(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        RemovePlayerTouch(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AddPlayerTouch(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        AddPlayerTouch(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RemovePlayerTouch(other);
    }

    private void AddPlayerTouch(Collider2D other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        isPlayerTouching = true;
        SetVisible(true);
    }

    private void RemovePlayerTouch(Collider2D other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        isPlayerTouching = false;

        if (hideWhenPlayerLeaves && !isPlayerTouching)
        {
            SetVisible(false);
        }
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null)
        {
            return;
        }

        foreach (Renderer blockRenderer in renderers)
        {
            if (blockRenderer != null)
            {
                blockRenderer.enabled = visible;
            }
        }
    }
}
