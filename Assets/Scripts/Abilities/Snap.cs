using UnityEngine;

public class Snap : RhythmInput
{
    [SerializeField] Animator snapAnim;
    [SerializeField] AudioClip snapClip;
    public override void EvaluateTiming()
    {
        base.EvaluateTiming();
        if (snapAnim)
        snapAnim.SetTrigger("Snap");
        AudioManager.Instance.PlaySound(snapClip);
    }
}
