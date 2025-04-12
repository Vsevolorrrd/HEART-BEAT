using System.Collections;
using UnityEngine;

public class Press : RespondToBeat
{
    [Header("Press Settings")]
    [SerializeField] Transform press;
    [SerializeField] Vector3 pressOffset = new Vector3(0, -2, 0);
    [SerializeField] float retractTime = 0.2f;

    private Vector3 pressRetractedPos;
    private Vector3 pressExtendedPos;

    private Coroutine retractionCoroutine;

    private void Start()
    {
        pressRetractedPos = press.localPosition;
        pressExtendedPos = pressRetractedPos + pressOffset;
    }

    protected override void OnBeat()
    {
        press.localPosition = pressExtendedPos;

        if (retractionCoroutine != null)
        StopCoroutine(retractionCoroutine);

        retractionCoroutine = StartCoroutine(Retract());
    }

    private IEnumerator Retract()
    {
        Vector3 startPos = press.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < retractTime)
        {
            press.localPosition = Vector3.Lerp(startPos, pressRetractedPos, elapsedTime / retractTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        press.localPosition = pressRetractedPos;
    }
}