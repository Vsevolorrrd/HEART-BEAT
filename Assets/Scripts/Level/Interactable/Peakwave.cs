using UnityEngine;

public class Peakwave : MonoBehaviour, IInteractable
{
    [SerializeField] float streakGain = 50f;
    public void Interact()
    {
        RhythmStreakManager.Instance.RegisterHit(streakGain);
        Destroy(gameObject);
    }
}
