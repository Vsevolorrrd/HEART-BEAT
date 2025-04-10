using System.Collections;
using UnityEngine;

public class SkyBeatBox : MonoBehaviour
{
    [SerializeField] float exposurePerBeat = 1f;
    [SerializeField] float startExposure = 1f;
    [SerializeField] float resetDuration = 0.2f;

    void Start()
    {
        if (RenderSettings.skybox.HasFloat("_Exposure"))
        RenderSettings.skybox.SetFloat("_Exposure", startExposure);

        BEAT_Manager.BEAT += OnBeat;
    }

    void OnBeat()
    {
        if (RenderSettings.skybox.HasFloat("_Exposure"))
        {
            float currentExposure = RenderSettings.skybox.GetFloat("_Exposure");
            RenderSettings.skybox.SetFloat("_Exposure", currentExposure + exposurePerBeat);
            StopAllCoroutines();
            StartCoroutine(ResetExposure());
        }
    }
    IEnumerator ResetExposure()
    {
        float elapsedTime = 0f;
        float currentExposure = RenderSettings.skybox.GetFloat("_Exposure");

        while (elapsedTime < resetDuration)
        {
            float t = elapsedTime / resetDuration;
            float exposure = Mathf.Lerp(currentExposure, startExposure, t);
            RenderSettings.skybox.SetFloat("_Exposure", exposure);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        RenderSettings.skybox.SetFloat("_Exposure", startExposure);
    }

    private void OnDestroy()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }
}
