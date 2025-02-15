using UnityEngine;
using System.Collections;

public class DashModule : RhythmInput
{
    private FPSController controller;
    private CharacterController charController;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private float dashCooldown = 0.5f;
    private float dashCooldownTimer;
    private float startDashSpeed;
    private bool isDashing = false;

    [SerializeField] AudioClip dashModuleClip;
    [SerializeField] ParticleSystem dashEffect;

    [Header("FOV Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] private float fovIncrease = 15f;
    [SerializeField] private float fovTransitionTime = 0.2f;


    public override void Start()
    {
        base.Start();

        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased += changePlayerStats;
        }

        controller = GetComponent<FPSController>();
        charController = GetComponent<CharacterController>();

        startDashSpeed = dashSpeed;
    }

    public override void Update()
    {
        if (!playerInput || isDashing) return;

        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    public override void OnPerfectHit()
    {
        base.OnPerfectHit();
        StartDash(1.5f);
    }

    public override void OnGoodHit()
    {
        base.OnGoodHit();
        StartDash(1);
    }

    private void StartDash(float modifier)
    {
        if (dashCooldownTimer > 0 || isDashing) return;

        dashCooldownTimer = dashCooldown;
        StartCoroutine(Dash(modifier));
    }

    private IEnumerator Dash(float modifier)
    {
        isDashing = true;
        AudioManager.Instance.PlaySound(dashModuleClip, 0.5f);
        if (dashEffect) dashEffect.Play();

        Vector3 direction = GetDashDirection();
        Vector3 dashVelocity = direction * dashSpeed * modifier;

        float elapsedTime = 0f;
        controller.AdjustFOV(fovIncrease, fovTransitionTime, fovTransitionTime);

        while (elapsedTime < dashDuration)
        {
            charController.Move(dashVelocity * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private Vector3 GetDashDirection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = transform.forward * verticalInput + transform.right * horizontalInput;

        if (direction.magnitude == 0)
        {
            direction = transform.forward;
        }

        return direction.normalized;
    }

    #region events

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased -= changePlayerStats;
        }
    }

    private void changePlayerStats(int level)
    {
        switch (level)
        {
            case 3:
                dashSpeed = startDashSpeed * 1.50f;
                break;
            case 2:
                dashSpeed = startDashSpeed * 1.25f;
                break;
            case 1:
                dashSpeed = startDashSpeed;
                break;
        }
    }

    #endregion
}