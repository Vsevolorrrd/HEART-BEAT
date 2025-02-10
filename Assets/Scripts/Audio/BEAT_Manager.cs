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
    [HideInInspector] public AudioClip footStepsClip;

    private float songBPM;
    private float nextBeat;
    private int musicLevel;
    public static event Action BEAT;
    public static event Action<int> MusicLevelIncreased;
    public static event Action<double> OnMusicStart;

    private static BEAT_Manager _instance;

    #region Singleton
    public static BEAT_Manager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("BEAT_Manager already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
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
        footStepsClip = song.footSteps;

        secPerBeat = 60f / songBPM;
        dspSongTime = (float)AudioSettings.dspTime;
        // AudioSettings.dspTime - a precision timer that represents the current time since the audio system started

        double startTime = AudioSettings.dspTime + 0.1;
        OnMusicStart?.Invoke(startTime); // Notify subscribers (if the sound should in sync with the music)

        mainMusicLevel.PlayScheduled(startTime);
        musicLevel_2.PlayScheduled(startTime);
        musicLevel_3.PlayScheduled(startTime);
        musicLevel_2.volume = 0;
        musicLevel_3.volume = 0;
        musicLevel_2.loop = true;
        musicLevel_3.loop = true;

        nextBeat = secPerBeat;
        MainMenu.OnPause += HandlePause;
    }
    void FixedUpdate()
    {
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
    }

    private IEnumerator TransitionMusicLevel(int targetLevel)
    {
        float targetVolume2 = (targetLevel >= 2) ? 1f : 0f;
        float targetVolume3 = (targetLevel >= 3) ? 1f : 0f;

        while (!Mathf.Approximately(musicLevel_2.volume, targetVolume2) ||
               !Mathf.Approximately(musicLevel_3.volume, targetVolume3))
        {
            musicLevel_2.volume = Mathf.MoveTowards(musicLevel_2.volume, targetVolume2, Time.deltaTime / transitionSpeed);
            musicLevel_3.volume = Mathf.MoveTowards(musicLevel_3.volume, targetVolume3, Time.deltaTime / transitionSpeed);
            yield return null;
        }
    }

    #region events

    private void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;
    }

    private void HandlePause(bool pause)
    {
        if (pause)
        {
            mainMusicLevel.volume = 0.3f;
            musicLevel_2.volume = 0.3f;
            musicLevel_3.volume = 0.3f;
        }
        else
        {
            mainMusicLevel.volume = 1f;
            musicLevel_2.volume = 1f;
            musicLevel_3.volume = 1f;
        }
    }
    #endregion
}