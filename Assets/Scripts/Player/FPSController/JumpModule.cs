public class JumpModule : RhythmInput
{
    private FPSController controller;
    protected override void Start()
    {
        controller = GetComponent<FPSController>();
    }
    protected override void Update()
    {
         return;
    }
    public void CheckJump()
    {
        HandleKeyPress();
    }
    protected override void OnPerfectHit()
    {
        controller.Jump();
    }
    protected override void OnGoodHit()
    {
        controller.Jump();
    }
}
