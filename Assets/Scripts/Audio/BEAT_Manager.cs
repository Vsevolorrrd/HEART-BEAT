using UnityEngine;

public class BEAT_Manager : MonoBehaviour
{
    [SerializeField] float songBPM;
    [SerializeField] float secPerBeat;
    [SerializeField] float songPosition;
    [SerializeField] float songPositionInBeats;
    [SerializeField] float dspSongTime;
    [SerializeField] AudioSource musicSource;
   
    public float GetsongPositionInBeats()
    {
        return songPositionInBeats;
    }
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        secPerBeat = 60f / songBPM;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        songPositionInBeats = songPosition / secPerBeat;
    }
}
