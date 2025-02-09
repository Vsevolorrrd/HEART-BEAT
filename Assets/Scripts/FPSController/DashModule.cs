using UnityEngine;
using System.Collections;

public class DashModule : RhythmInput
{
    private Rigidbody rb;

    public float dashForce = 70f;
    public float dashUpwardForce = 2f;
    public float dashCooldown = 0.5f;
    private float dashCooldownTimer;
    private float startDashForce;
    [SerializeField] AudioClip dashModuleClip;
    [SerializeField] ParticleSystem dashEfect;

    [Header("FOV Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] private float fovIncrease = 15f;
    [SerializeField] private float fovTransitionTime = 0.2f;

    private float defaultFOV;

    public override void Start()
    {
        base.Start();

        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased += changePlayerStats;
        }

        rb = GetComponent<Rigidbody>();
        startDashForce = dashForce;
        defaultFOV = playerCam.fieldOfView;
    }
    public override void Update()
    {
        if (!playerInput) 
        return;

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

        AudioManager.Instance.PlaySound(dashModuleClip, 0.5f);
        if (dashEfect)
        dashEfect.Play();
        StartCoroutine(ChangeFOV(defaultFOV + fovIncrease, fovTransitionTime));

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
    private IEnumerator ChangeFOV(float targetFOV, float transitionTime)
    {
        
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, 10f * Time.deltaTime);
        
        yield return new WaitForSeconds(transitionTime / 2);

        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, defaultFOV, 10f * Time.deltaTime);
        
        yield return new WaitForSeconds(transitionTime / 2);

        playerCam.fieldOfView = defaultFOV;
    }

    #region events

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased += changePlayerStats;
        }
    }
    private void changePlayerStats(int level)
    {
        switch (level)
        {
            case 3:
                dashForce = startDashForce * 1.50f;
                break;
            case 2:
                dashForce = startDashForce * 1.25f;
                break;
            case 1:
                dashForce = startDashForce;
                break;
        }
    }

    #endregion

}