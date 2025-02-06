using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFXPrefab;
    private List<AudioSource> loopSources;

    private static AudioManager _instance;

    #region Singleton
    public static AudioManager Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing AudioManager in the scene
                _instance = FindAnyObjectByType<AudioManager>();

                // If no AudioManager exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("AudioManager");
                    _instance = singletonObject.AddComponent<AudioManager>();
                }

                // Make the AudioManager persist across scenes (optional)
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        // If the instance is already set, destroy this duplicate object
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;  // Assign this object as the instance
        }
    }
    #endregion

    public void PlaySound(AudioClip audioClip, Transform spawn = null, float volume = 1, bool loop = false)
    {
        if (spawn == null)
            spawn = transform; // Default to this object's transform
        AudioSource audioSource = Instantiate(soundFXPrefab, spawn.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRandomSound(AudioClip[] audioClip, Transform spawn = null, float volume = 1, bool loop = false)
    {
        int R = Random.Range(0, audioClip.Length);

        if (spawn == null)
            spawn = transform; // Default to this object's transform
        AudioSource audioSource = Instantiate(soundFXPrefab, spawn.position, Quaternion.identity);

        audioSource.clip = audioClip[R];
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();

        if (loop)
        {
            loopSources.Add(audioSource);
            return;
        }

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayMusic(AudioClip audioClip, Transform spawn = null, float volume = 1, bool loop = true)
    {
        if (spawn == null)
            spawn = transform; // Default to this object's transform
        AudioSource audioSource = Instantiate(soundFXPrefab, spawn.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void StopSoundGradually(AudioClip audioClip, float fadeDuration = 2f)
    {
        StartCoroutine(FadeOutSound(audioClip, fadeDuration));
    }
    public IEnumerator FadeOutSound(AudioClip audioClip, float duration)
    {
        AudioSource audioSource = loopSources.Find(sources => sources.clip == audioClip);
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        Destroy(audioSource.gameObject);
    }
}
