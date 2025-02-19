using UnityEngine;

public class SkyBeatBox : MonoBehaviour
{
    public Skybox skyboxMaterial;
    private float beatPulse = 0f;
    private float decaySpeed = 3f;

    void Start()
    {
        BEAT_Manager.BEAT += OnBeat;
    }

    void Update()
    {
        if (skyboxMaterial)
        {
            beatPulse = Mathf.Lerp(beatPulse, 0f, Time.deltaTime * decaySpeed);
        }
    }

    void OnBeat()
    {
        beatPulse = 1.5f;
    }

    private void OnDestroy()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }
}
