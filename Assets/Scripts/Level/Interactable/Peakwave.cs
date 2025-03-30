using UnityEngine;

public class Peakwave : RespondToBeat, IInteractable
{
    [SerializeField] float streakGain = 50f;
    public void Interact()
    {
        RhythmStreakManager.Instance.RegisterHit(streakGain);
        Destroy(gameObject);
    }
    protected override void OnBeat()
    {
        // Scale the object up
        transform.localScale = Vector3.one * 1.2f;

        // Reset scale after 0.1s
        Invoke(nameof(ResetScale), 0.1f);
    }

    private void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}
