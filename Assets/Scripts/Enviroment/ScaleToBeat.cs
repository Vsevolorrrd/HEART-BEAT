using UnityEngine;

public class ScaleToBeat : RespondToBeat
{
    [Header("Scaling Settings")]
    public float scaleMultiplier = 1.2f;
    public float scaleBackSpeed = 0.1f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleMultiplier;
    }

    public override void OnBeat()
    {
        // Scale the object up
        transform.localScale = targetScale;

        // Reset scale after 0.1s
        Invoke(nameof(ResetScale), scaleBackSpeed);
    }

    private void ResetScale()
    {
        transform.localScale = originalScale;
    }
        //stencil shader

}