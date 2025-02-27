using UnityEngine;

public class SkyBeatBox : MonoBehaviour
{
    void Start()
    {
        BEAT_Manager.BEAT += OnBeat;
    }

    void OnBeat()
    {

    }

    private void OnDestroy()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }
}
