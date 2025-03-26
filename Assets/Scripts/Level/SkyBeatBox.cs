using UnityEngine;

public class SkyBeatBox : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    private void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Get the current skybox material and apply the rotation
        if (RenderSettings.skybox.HasFloat("_Rotation"))
        RenderSettings.skybox.SetFloat("_Rotation", RenderSettings.skybox.GetFloat("_Rotation") + rotationAmount);
    }
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
