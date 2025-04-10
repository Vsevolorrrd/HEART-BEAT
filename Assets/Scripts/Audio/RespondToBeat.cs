using UnityEngine;

public abstract class RespondToBeat : MonoBehaviour
{
    private void OnEnable()
    {
        BEAT_Manager.BEAT += OnBeat;
    }

    private void OnDisable()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }

    protected abstract void OnBeat();
}
