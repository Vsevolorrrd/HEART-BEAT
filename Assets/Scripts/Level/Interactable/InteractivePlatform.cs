public class InteractivePlatform : MoveOnBeat, IInteractable
{
    private bool canMove = false;
    public void Interact()
    {
        canMove = true;
    }
    protected override void OnBeat()
    {
        if (!canMove) return;

        if (timesMoved >= timesToMove)
        {
            canMove = false;
            opositeWay = !opositeWay;
            timesMoved = 0;
            return;
        }

        timesMoved++;

        if (moveRoutine != null)
        StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToTarget());
    }
}