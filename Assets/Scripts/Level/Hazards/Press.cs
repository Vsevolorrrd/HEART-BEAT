using System.Collections;
using UnityEngine;

public class Press : RespondToBeat
{
    [Header("Press Settings")]
    public float scaleMultiplier = 1.2f;
    public float scaleSpeed = 0.1f;
    private bool dangerous = false;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = new Vector3(originalScale.x, originalScale.y * scaleMultiplier, originalScale.z);
    }

    public override void OnBeat()
    {
        transform.localScale = targetScale;
        dangerous = true;
        StartCoroutine(ScaleObject(originalScale, scaleSpeed));
    }

    private IEnumerator ScaleObject(Vector3 target, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dangerous = false;

        transform.localScale = target;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (dangerous)
        {
            FPSController player = other.GetComponent<FPSController>();
            if (player)
            {
                player.transform.position = new Vector3(0, 2, 0);
                player.ResetVilocity(0);
            }
        }
    }
}
