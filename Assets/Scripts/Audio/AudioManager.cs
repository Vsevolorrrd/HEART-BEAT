using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFXPrefab;
    private List<AudioSource> loopSources;
    private List<AudioSource> audioPool = new List<AudioSource>();

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
        InitializeAudioPool();
    }
    #endregion

    public void PlayPooledSound(AudioClip clip, float volume = 1f, float pitch = 1f) // for sounds that should be played instantly (basically player actions)
    {
        AudioSource audioSource = GetPooledAudioSource();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        StartCoroutine(DeactivateAfterPlay(audioSource));
    }
    private void InitializeAudioPool()
    {
        for (int i = 0; i < 15; i++)  // Pool size of 15
        {
            AudioSource audioSource = Instantiate(soundFXPrefab, transform);
            audioSource.gameObject.SetActive(false);
            audioPool.Add(audioSource);
        }
    }
    private AudioSource GetPooledAudioSource()
    {
        foreach (AudioSource source in audioPool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        // Expand pool if needed
        AudioSource newSource = Instantiate(soundFXPrefab, transform);
        audioPool.Add(newSource);
        return newSource;
    }
    private IEnumerator DeactivateAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.gameObject.SetActive(false);
    }
    public void PlaySound(AudioClip audioClip, float volume = 1, Transform spawn = null, bool loop = false, float pitch = 1f)
    {
        if (spawn == null)
        spawn = transform; // Default to this object's transform
        AudioSource audioSource = Instantiate(soundFXPrefab, spawn.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        audioSource.Play();

        if (loop)
        {
            loopSources.Add(audioSource);
            return;
        }

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRandomSound(AudioClip[] audioClip, float volume = 1, Transform spawn = null, bool loop = false, float pitch = 1f)
    {
        int R = Random.Range(0, audioClip.Length);

        if (spawn == null)
        spawn = transform; // Default to this object's transform
        AudioSource audioSource = Instantiate(soundFXPrefab, spawn.position, Quaternion.identity);

        audioSource.clip = audioClip[R];
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        audioSource.Play();

        if (loop)
        {
            loopSources.Add(audioSource);
            return;
        }

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
    public void StopAllLoopSources(float fadeDuration = 1f)
    {
        FadeOutAllLoopSources(fadeDuration);
    }
    public IEnumerator FadeOutAllLoopSources(float duration)
    {
        foreach (AudioSource audioSource in loopSources)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }
            Destroy(audioSource.gameObject);
        }
    }
}