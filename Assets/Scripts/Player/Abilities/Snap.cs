using UnityEngine;

public class Snap : RhythmInput
{
    [SerializeField] Animator snapAnim;
    [SerializeField] AudioClip snapClip;
    protected override void EvaluateTiming()
    {
        base.EvaluateTiming();
        if (snapAnim)
        snapAnim.SetTrigger("Snap");
        AudioManager.Instance.PlayPooledSound(snapClip, 0.7f);
    }
}
