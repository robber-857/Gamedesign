using UnityEngine;

public class LevelAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = volume;

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
        }
    }

    private void Start()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
