using System.Collections;
using UnityEngine;

public class MoveOnBeat : RespondToBeat
{
    [SerializeField] protected Vector3 moveOffset = new Vector3(3, 0, 0); // How much the object moves per beat
    [SerializeField] protected int timesToMove = 6;
    protected int timesMoved;
    protected bool opositeWay = false;
    protected float moveTime = 0.2f;
    protected Coroutine moveRoutine;

    protected override void OnBeat()
    {
        if (timesMoved >= timesToMove)
        {
            opositeWay = !opositeWay;
            timesMoved = 0;
        }

        timesMoved++;

        if (moveRoutine != null)
        StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToTarget());
    }

    protected IEnumerator MoveToTarget()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = opositeWay ? startPos - moveOffset : startPos + moveOffset;
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}