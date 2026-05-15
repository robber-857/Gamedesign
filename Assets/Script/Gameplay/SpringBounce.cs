using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class SpringBounce : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float bounceVelocity = 13f;
    [SerializeField] private bool requirePlayerAboveSpring = true;
    [SerializeField] private bool resetDownwardVelocityOnly = true;
    [SerializeField] private Animator springAnimator;
    [SerializeField] private string bounceAnimationState = "New Animation";
    [SerializeField] private float animationDuration = 0.34f;
    [SerializeField] private AudioSource bounceAudioSource;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private string fallbackBounceSoundResourcePath = "Audio/spring_bounce";

    private Collider2D springCollider;
    private bool isPlayerTouching;
    private Coroutine animationRoutine;
    private static AudioClip cachedFallbackBounceSound;

    private void Awake()
    {
        springCollider = GetComponent<Collider2D>();

        if (springAnimator == null)
        {
            springAnimator = GetComponent<Animator>();
        }

        if (bounceAudioSource == null)
        {
            bounceAudioSource = GetComponent<AudioSource>();
        }

        if (bounceSound == null && bounceAudioSource != null)
        {
            bounceSound = bounceAudioSource.clip;
        }

        ResetSpringAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryBounce(collision.collider, collision.rigidbody);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag(playerTag))
        {
            isPlayerTouching = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryBounce(other, other.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerTouching = false;
        }
    }

    private void TryBounce(Collider2D playerCollider, Rigidbody2D playerBody)
    {
        if (isPlayerTouching || playerCollider == null || !playerCollider.CompareTag(playerTag))
        {
            return;
        }

        if (playerBody == null)
        {
            playerBody = playerCollider.GetComponentInParent<Rigidbody2D>();
        }

        if (playerBody == null || !IsValidBounceContact(playerCollider, playerBody))
        {
            return;
        }

        Vector2 velocity = playerBody.linearVelocity;

        if (!resetDownwardVelocityOnly || velocity.y < bounceVelocity)
        {
            isPlayerTouching = true;
            velocity.y = bounceVelocity;
            playerBody.linearVelocity = velocity;
            PlayBounceSound();
            PlaySpringAnimationOnce();
        }
    }

    private void PlayBounceSound()
    {
        AudioClip clip = GetBounceSound();

        if (clip == null)
        {
            return;
        }

        if (bounceAudioSource == null)
        {
            bounceAudioSource = gameObject.AddComponent<AudioSource>();
            bounceAudioSource.playOnAwake = false;
        }

        bounceAudioSource.PlayOneShot(clip);
    }

    private AudioClip GetBounceSound()
    {
        if (bounceSound != null)
        {
            return bounceSound;
        }

        if (cachedFallbackBounceSound == null && !string.IsNullOrWhiteSpace(fallbackBounceSoundResourcePath))
        {
            cachedFallbackBounceSound = Resources.Load<AudioClip>(fallbackBounceSoundResourcePath);
        }

        return cachedFallbackBounceSound;
    }

    private bool IsValidBounceContact(Collider2D playerCollider, Rigidbody2D playerBody)
    {
        if (!requirePlayerAboveSpring || springCollider == null)
        {
            return true;
        }

        bool playerFeetAreAboveSpringCenter = playerCollider.bounds.min.y >= springCollider.bounds.center.y;
        bool playerIsMovingDownOrResting = playerBody.linearVelocity.y <= 0.5f;
        return playerFeetAreAboveSpringCenter && playerIsMovingDownOrResting;
    }

    private void PlaySpringAnimationOnce()
    {
        if (springAnimator == null)
        {
            return;
        }

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        animationRoutine = StartCoroutine(PlaySpringAnimationRoutine());
    }

    private IEnumerator PlaySpringAnimationRoutine()
    {
        springAnimator.speed = 1f;
        springAnimator.Play(bounceAnimationState, 0, 0f);
        springAnimator.Update(0f);

        yield return new WaitForSeconds(animationDuration);

        ResetSpringAnimation();
        animationRoutine = null;
    }

    private void ResetSpringAnimation()
    {
        if (springAnimator == null)
        {
            return;
        }

        springAnimator.Play(bounceAnimationState, 0, 0f);
        springAnimator.Update(0f);
        springAnimator.speed = 0f;
    }
}
