using UnityEngine;
using System.Collections;

public class DashModule : RhythmInput
{
    private FPSController controller;
    private PlayerHealth playerHealth;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private float startDashSpeed;
    private bool isDashing = false;

    [SerializeField] ParticleSystem dashEffect;

    [Header("FOV Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] private float fovIncrease = 15f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip dashModuleClip;


    protected override void Start()
    {
        BEAT_Manager.MusicLevelIncreased += changePlayerStats;
        controller = GetComponent<FPSController>();
        playerHealth = GetComponent<PlayerHealth>();
        startDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        if (!PlayerManager.Instance.playerInput || !controller.isMoving || isDashing) return;

        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }
    }

    protected override void OnPerfectHit()
    {
        StartCoroutine(Dash(1.2f));
    }

    protected override void OnGoodHit()
    {
        StartCoroutine(Dash(1f));
    }

    private IEnumerator Dash(float modifier)
    {
        isDashing = true;
        playerHealth.SetVulnerability(false);
        AudioManager.Instance.PlaySound(dashModuleClip, 0.7f);
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
        playerHealth.SetVulnerability(true);
    }

    #region events

    protected  void OnDestroy()
    {
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