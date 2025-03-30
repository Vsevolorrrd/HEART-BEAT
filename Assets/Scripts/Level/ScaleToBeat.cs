using System.Collections;
using UnityEngine;

public class ScaleToBeat : RespondToBeat
{
    [Header("Scaling Settings")]
    public bool scaleSmooth = true;
    public float scaleMultiplier = 1.2f;
    public float scaleBackSpeed = 0.1f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleMultiplier;
    }

    protected override void OnBeat()
    {
        // Scale the object up
        transform.localScale = targetScale;

        if (scaleSmooth )
        {
            StartCoroutine(ScaleObject(originalScale, scaleBackSpeed));
        }
        else
        {
            Invoke(nameof(ResetScale), scaleBackSpeed);
        }
    }

    private void ResetScale()
    {
        transform.localScale = originalScale;
    }
    public IEnumerator ScaleObject(Vector3 target, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;
    }

}