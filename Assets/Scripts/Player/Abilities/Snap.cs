using UnityEngine;

public class Snap : RhythmInput
{
    [Header("Snap")]
    [SerializeField] Animator snapAnim;
    [SerializeField] AudioClip snapClip;
    [SerializeField] float snapShowDuration = 2f;
    private bool snapped = false;
    private float timer;

    public bool IsBlocked() {  return isBlocked; }
    protected override void Update()
    {
        if (snapped)
        {
            timer += Time.deltaTime;

            if (timer >= snapShowDuration)
            {
                if (snapAnim)
                snapAnim.SetBool("Show", false);
                snapped = false;
                timer = 0f;
            }
        }

    }
    public void PerfomSnap()
    {
        HandleKeyPress();

        snapped = true;
        timer = 0f;

        if (snapAnim)
        {
            snapAnim.SetBool("Show", true);
            snapAnim.SetTrigger("Snap");
        }

        AudioManager.Instance.PlayPooledSound(snapClip, 0.8f);
    }
}