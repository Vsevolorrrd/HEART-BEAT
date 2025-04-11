using UnityEngine;
using System.Collections;

public class SpinOnBeat : RespondToBeat
{
    [SerializeField] protected Vector3 rotationAmount = new Vector3(0, 90, 0); // How much to rotate per beat
    [SerializeField] protected float rotationTime = 0.2f;
    protected Coroutine rotateRoutine;

    protected override void OnBeat()
    {
        if (rotateRoutine != null)
        StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateToTarget());
    }

    protected IEnumerator RotateToTarget()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + rotationAmount);
        float elapsedTime = 0;

        while (elapsedTime < rotationTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}