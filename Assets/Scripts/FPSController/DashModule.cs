using UnityEngine;
using System.Collections;

public class DashModule : RhythmInput
{
    private FPSController controller;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private float startDashSpeed;
    private bool isDashing = false;

    [SerializeField] AudioClip dashModuleClip;
    [SerializeField] ParticleSystem dashEffect;

    [Header("FOV Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] private float fovIncrease = 15f;


    public override void Start()
    {
        base.Start();

        BEAT_Manager.MusicLevelIncreased += changePlayerStats;
        controller = GetComponent<FPSController>();
        startDashSpeed = dashSpeed;
    }

    public override void Update()
    {
        if (!playerInput || !controller.isMoving || isDashing) return;

        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }
    }

    public override void OnPerfectHit()
    {
        StartCoroutine(Dash(1.2f));
    }

    public override void OnGoodHit()
    {
        StartCoroutine(Dash(1f));
    }

    private IEnumerator Dash(float modifier)
    {
        isDashing = true;
        AudioManager.Instance.PlaySound(dashModuleClip, 0.5f);
        if (dashEffect) dashEffect.Play();

        controller.ChangeSpeed(dashSpeed * modifier);

        float elapsedTime = 0f;
        controller.AdjustFOV(fovIncrease, dashDuration, dashDuration);

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        controller.ChangeSpeed(-dashSpeed * modifier);
        isDashing = false;
    }

    #region events

    public override void OnDestroy()
    {
        base.OnDestroy();
        BEAT_Manager.MusicLevelIncreased -= changePlayerStats;
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