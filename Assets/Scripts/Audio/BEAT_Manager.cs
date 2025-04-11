using System.Collections;
using UnityEngine;
using System;

public class BEAT_Manager : MonoBehaviour
{
    [SerializeField] Music song;
    [SerializeField] float secPerBeat;
    [SerializeField] float songPositionInSeconds;
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
    [SerializeField] float nextBeatInSeconds;
    private int musicLevel;
    private bool musicStarted = false;

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
    public float GetSongBPM() => songBPM;
    public int GetMusicLevel() => musicLevel;


    public void StartTheMusic()
    {
        if (musicStarted || song == null)
        {
            Debug.LogWarning("Music already started or song is missing.");
            return;
        }

        songBPM = song.songBPM;
        mainMusicLevel.clip = song.Leadingtrack;
        musicLevel_2.clip = song.track_2;
        musicLevel_3.clip = song.track_3;
        transition = song.transition;

        secPerBeat = 60f / songBPM;
        dspSongTime = (float)AudioSettings.dspTime;

        StartAudio();
        nextBeatInSeconds = secPerBeat;
        musicStarted = true;

        PauseMenu.OnPause += HandlePause;
    }
    private void StartAudio()
    {
        double startTime = AudioSettings.dspTime + 0.2; // Schedule slightly ahead

        mainMusicLevel.PlayScheduled(startTime);
        musicLevel_2.PlayScheduled(startTime);
        musicLevel_3.PlayScheduled(startTime);

        dspSongTime = (float)startTime;
        OnMusicStart?.Invoke(startTime); // Notify subscribers (if the sound should in sync with the music)

        musicLevel_2.volume = 0;
        musicLevel_3.volume = 0;
    }
    void Update()
    {
        if (isPaused) return;

        songPositionInSeconds = (float)(AudioSettings.dspTime - dspSongTime);
        songPositionInBeats = songPositionInSeconds / secPerBeat;

        // Trigger BEAT event when the song reaches the next beat
        if (songPositionInSeconds >= nextBeatInSeconds)
        {
            BEAT?.Invoke();
            nextBeatInSeconds = Mathf.Ceil(songPositionInSeconds / secPerBeat) * secPerBeat;
            // Mathf.Ceil - rounds a number up
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

            musicLevel_2.volume = Mathf.Lerp(musicLevel_2.volume, targetVolume2, elapsedTime / transitionSpeed);
            musicLevel_3.volume = Mathf.Lerp(musicLevel_3.volume, targetVolume3, elapsedTime / transitionSpeed);

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
    public void SetNewMusic(Music newSong)
    {
        if (musicStarted)
        {
            StopMusic();
        }

        song = newSong;
    }
    public void StopMusic()
    {
        mainMusicLevel.Stop();
        musicLevel_2.Stop();
        musicLevel_3.Stop();
        musicStarted = false;
    }

    #region events

    private void OnDestroy()
    {
        PauseMenu.OnPause -= HandlePause;
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