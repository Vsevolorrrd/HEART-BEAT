using UnityEngine;

public class DashModule : RhythmInput
{
    private Rigidbody rb;
    private FPSController fpsController;

    public float dashForce = 50f;
    public float dashUpwardForce = 2f;
    public float dashCooldown = 0.5f;
    private float dashCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fpsController = GetComponent<FPSController>();
    }
    public override void Update()
    {
        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }

        if (dashCooldownTimer > 0)
        dashCooldownTimer -= Time.deltaTime;
    }
    public override void OnPerfectHit()
    {
        base.OnPerfectHit();
        Dash(1.50f);
    }
    public override void OnGoodHit()
    {
        base.OnGoodHit();
        Dash(1);
    }
    private void Dash(float modifier)
    {
        if (dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;

        Vector3 direction = GetDirection(transform);

        Vector3 forceToApply = direction * dashForce * modifier + transform.up * dashUpwardForce;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

}
