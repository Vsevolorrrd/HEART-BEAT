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

    private float songBPM;
    private float nextBeat;
    public static event Action BEAT;
    public event Action<double> OnMusicStart; // Event to share the start time

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

    void Start()
    {
        songBPM = song.songBPM;
        mainMusicLevel.clip = song.Leadingtrack;
        musicLevel_2.clip = song.track_2;
        musicLevel_3.clip = song.track_3;

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
    }
    void Update()
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
    public void LevelUPMusic()
    {
        if (musicLevel_2.volume == 1)
        {
            if (musicLevel_3.volume == 1)
            return;

            IncreaseVolumeGradually(musicLevel_3);
        }
        else
        {
            IncreaseVolumeGradually(musicLevel_2);
        }
    }
    public IEnumerator IncreaseVolumeGradually(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume < 1)
        {
            audioSource.volume -= startVolume * Time.deltaTime / transitionSpeed;
            yield return null;
        }
    }
    public void DecreaseMusicLevel()
    {
        if (musicLevel_3.volume == 1)
        {
            if (musicLevel_2.volume == 1)
            DecreaseVolumeGradually(musicLevel_2);
            else
            DecreaseVolumeGradually(musicLevel_3);

        }
    }
    public IEnumerator DecreaseVolumeGradually(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / transitionSpeed;
            yield return null;
        }
    }
}