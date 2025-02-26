using System.Collections;
using UnityEngine;
using System;

public class BEAT_Manager : MonoBehaviour
{
    [SerializeField] Music song;
    [SerializeField] float secPerBeat;
    [SerializeField] float songPosition;
    [SerializeField] float songPositionInBeats;
    [SerializeField] float dspSongTime;

    [Header("SongLevels")]
    [SerializeField] AudioSource mainMusicLevel;
    [SerializeField] AudioSource musicLevel_2;
    [SerializeField] AudioSource musicLevel_3;
    [SerializeField] float transitionSpeed = 1f;
    private AudioClip transition;

    // Pause Settings
    private float pauseTimeOffset = 0f;
    private bool isPaused = false;

    private float songBPM;
    private float nextBeat;
    private int musicLevel;

    public static event Action BEAT;
    public static event Action<int> MusicLevelIncreased;
    public static event Action<double> OnMusicStart;

    private static BEAT_Manager _instance;

    #region Singleton
    public static BEAT_Manager Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("BEAT_Manager already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    public float GetSongPositionInBeats() => songPositionInBeats;
    public float GetSecPerBeat() => secPerBeat;
    public int GetMusicLevel() => musicLevel;


    void Start()
    {
        songBPM = song.songBPM;
        mainMusicLevel.clip = song.Leadingtrack;
        musicLevel_2.clip = song.track_2;
        musicLevel_3.clip = song.track_3;
        transition = song.transition;

        secPerBeat = 60f / songBPM;
        dspSongTime = (float)AudioSettings.dspTime;
        // AudioSettings.dspTime - a precision timer that represents the current time since the audio system started

        StartCoroutine(StartAudioAfterFrame());

        nextBeat = secPerBeat;
        MainMenu.OnPause += HandlePause;
    }
    private IEnumerator StartAudioAfterFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for audio engine to stabilize

        double startTime = AudioSettings.dspTime + 0.1; // Schedule slightly ahead
        dspSongTime = (float)startTime;

        Debug.Log($"[BEAT_Manager] Starting music at DSP time: {dspSongTime}");

        mainMusicLevel.PlayScheduled(startTime);
        musicLevel_2.PlayScheduled(startTime);
        musicLevel_3.PlayScheduled(startTime);
        OnMusicStart?.Invoke(startTime); // Notify subscribers (if the sound should in sync with the music)

        musicLevel_2.volume = 0;
        musicLevel_3.volume = 0;
    }
    void Update()
    {
        if (isPaused) return;

        songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        songPositionInBeats = songPosition / secPerBeat;

        // Trigger BEAT event when the song reaches the next beat
        if (songPosition >= nextBeat)
        {
            BEAT?.Invoke();
            nextBeat = Mathf.Floor(songPosition / secPerBeat + 1) * secPerBeat;
        }
    }
    public void SetMusicLevel(int level)
    {
        musicLevel = level;
        MusicLevelIncreased?.Invoke(level);
        StartCoroutine(TransitionMusicLevel(level));
        if (transition)
        AudioManager.Instance.PlayPooledSound(transition);
    }

    private IEnumerator TransitionMusicLevel(int targetLevel)
    {
        float targetVolume2 = (targetLevel >= 2) ? 1f : 0f;
        float targetVolume3 = (targetLevel >= 3) ? 1f : 0f;

        float elapsedTime = 0f;
        float maxTransitionTime = transitionSpeed * 2f; // Failsafe timeout

        while (elapsedTime < maxTransitionTime)
        {
            elapsedTime += Time.deltaTime;

            musicLevel_2.volume = Mathf.Lerp(musicLevel_2.volume, targetVolume2, Time.deltaTime / transitionSpeed);
            musicLevel_3.volume = Mathf.Lerp(musicLevel_3.volume, targetVolume3, Time.deltaTime / transitionSpeed);

            // If both volumes are close to target, exit early
            if (Mathf.Abs(musicLevel_2.volume - targetVolume2) < 0.01f &&
                Mathf.Abs(musicLevel_3.volume - targetVolume3) < 0.01f)
            {
                break;
            }

            yield return new WaitForSecondsRealtime(0.02f);
        }

        // Ensure exact final values
        musicLevel_2.volume = targetVolume2;
        musicLevel_3.volume = targetVolume3;
    }

    #region events

    private void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;
    }

    private void HandlePause(bool pause)
    {
        isPaused = pause;

        if (pause)
        {
            pauseTimeOffset = (float)(AudioSettings.dspTime - dspSongTime); // Save current song time

            mainMusicLevel.Pause();
            musicLevel_2.Pause();
            musicLevel_3.Pause();
        }
        else
        {
            dspSongTime = (float)AudioSettings.dspTime - pauseTimeOffset; // Adjust song start time

            mainMusicLevel.UnPause();
            musicLevel_2.UnPause();
            musicLevel_3.UnPause();
        }
    }
    #endregion
}