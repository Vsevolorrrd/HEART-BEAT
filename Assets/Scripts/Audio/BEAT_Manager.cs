using UnityEngine;
using System;

public class BEAT_Manager : MonoBehaviour
{
    [SerializeField] Music song;
    [SerializeField] float secPerBeat;
    [SerializeField] float songPosition;
    [SerializeField] float songPositionInBeats;
    [SerializeField] float dspSongTime;
    [SerializeField] AudioSource PrimaryMusicSource;
    [SerializeField] AudioSource musicSource_2;
    [SerializeField] AudioSource musicSource_3;

    private float songBPM;
    private float nextBeat;
    public event Action BEAT;
    public event Action<double> OnMusicStart; // Event to share the start time

    private static BEAT_Manager _instance;

    #region Singleton
    public static BEAT_Manager Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing beat manager in the scene
                _instance = FindAnyObjectByType<BEAT_Manager>();

                // If no beat manager exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("BEAT_Manager");
                    _instance = singletonObject.AddComponent<BEAT_Manager>();
                }

                // Make the beat manager persist across scenes (optional)
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

    public float GetSongPositionInBeats() => songPositionInBeats;
    public float GetSecPerBeat() => secPerBeat;

    void Start()
    {
        songBPM = song.songBPM;
        PrimaryMusicSource.clip = song.Leadingtrack;
        musicSource_2.clip = song.track_2;
        musicSource_3.clip = song.track_3;

        secPerBeat = 60f / songBPM;
        dspSongTime = (float)AudioSettings.dspTime; // A precision timer that represents the current time since the audio system started

        double startTime = AudioSettings.dspTime + 0.1;
        OnMusicStart?.Invoke(startTime); // Notify subscribers (like footsteps script)

        PrimaryMusicSource.PlayScheduled(startTime);
        musicSource_2.PlayScheduled(startTime);
        musicSource_3.PlayScheduled(startTime);
        musicSource_2.volume = 0;
        musicSource_3.volume = 0;
        musicSource_2.loop = true;
        musicSource_3.loop = true;

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
}