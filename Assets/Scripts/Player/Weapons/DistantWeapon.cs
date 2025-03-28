using UnityEngine;

public class DistantWeapon : RhythmInput
{
    protected Animator anim;

    [Header("Audio")]
    [SerializeField] protected AudioClip shotSound;
    [SerializeField] protected AudioClip perfectShot;
    [SerializeField] protected AudioClip startReload;
    [SerializeField] protected AudioClip reload;
    [SerializeField] protected AudioClip perfectReload;

    [Header("Stats")]
    [SerializeField] protected Camera cam;
    [SerializeField] protected float range = 100f;
    [SerializeField] protected LayerMask layerMaskToHit;
    [SerializeField] protected float damage = 25f;
    [SerializeField] protected int bulletsPerShot = 1, maxAmmo = 6;
    [SerializeField] protected float spread = 0f;
    protected int currentAmmo;

    [Header("Visual Effects")]
    [SerializeField] protected GameObject impact, projectilePrefab;
    [SerializeField] private ParticleSystem gunSmoke, muzzleFlash;
    [SerializeField] private Transform gunpoint;

    [Header("Camera Shake")]
    [SerializeField] private bool cameraShake = true;
    [SerializeField] private float shakeDuration = 0.2f, shakeAmplitude = 1f, shakeFrequency = 1f;

    [Header("Reload")]
    [SerializeField] protected KeyCode reloadKey = KeyCode.R;
    protected bool isReloading = false;

    [Header("Debug")]
    [SerializeField] protected float nextShotMinTime = 0;

    protected override void Start()
    {
        MainMenu.OnPause += HandlePause;
        anim = GetComponent<Animator>();
        currentAmmo = maxAmmo;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
    }

    protected override void Update()
    {
        if (!playerInput || isBlocked) return;

        if (Input.GetKeyDown(reloadKey))
        {
            if (currentAmmo < maxAmmo)
            {
                if (isReloading) HandleKeyPress();
                else StartReload();
            }
        }

        if (isReloading) return;

        if (Input.GetKeyDown(actionKey))
        {
            if (currentAmmo > 0) HandleKeyPress();
            else StartReload();
        }
    }

    protected override void HandleKeyPress()
    {
        float currentTime = Time.time;
        if (currentTime - firstPressTime > pressWindow)
        {
            firstPressTime = currentTime;
            pressCount = 0;
        }

        pressCount++;
        if (pressCount >= maxPresses)
        {
            StartCoroutine(BlockInput());
            return;
        }

        if (isReloading)
        {
            EvaluateReloadTiming();
            return;
        }

        Shoot();
    }

    protected virtual void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
        AudioManager.Instance.PlayPooledSound(shotSound, 0.9f);
        Effects();

        if (currentAmmo <= 0) StartReload();
    }

    protected void Effects()
    {
        anim.SetTrigger("Shot");

        if (cameraShake)
        CameraShake.Instance.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
        if (muzzleFlash)
        muzzleFlash.Play();
        if (gunSmoke)
        gunSmoke.Play();
    }

    protected Vector3 GetSpreadDirection()
    {
        return cam.transform.forward + new Vector3(
        Random.Range(-spread, spread),
        Random.Range(-spread, spread),
        Random.Range(-spread, spread));
    }

    protected void EvaluateTimingTarget(Damageable target)
    {
        float songPositionInBeats = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPositionInBeats); // Nearest beat
        float timeDifference = Mathf.Abs(songPositionInBeats - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= RhythmDifficulty.perfectThreshold)
        {
            OnPerfectShot(target);
        }
        else if (timeDifference <= RhythmDifficulty.goodThreshold)
        {
            OnGoodShot(target);
        }
        else
        {
            OnMissShot(target);
        }
    }

    protected virtual void OnPerfectShot(Damageable target)
    {
        HitEffect.Instance.playHitEffect("Perfect");
        BeatUI.Instance.ShowHitFeedback("Perfect");
        RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
        AudioManager.Instance.PlayPooledSound(perfectShot, 0.4f);
        target.Damage(damage * 1.5f);
    }

    protected virtual void OnGoodShot(Damageable target)
    {
        HitEffect.Instance.playHitEffect("Good");
        BeatUI.Instance.ShowHitFeedback("Good");
        RhythmStreakManager.Instance.RegisterHit(streakGainGood);
        target.Damage(damage);
    }

    protected virtual void OnMissShot(Damageable target)
    {
        BeatUI.Instance.ShowHitFeedback("Miss");
        target.Damage(damage * 0.5f);
    }

    #region Reload
    protected virtual void StartReload()
    {
        isReloading = true;
        anim.SetBool("StartReload", true);
        WeaponUI.Instance.ShowReloadUI();
        AudioManager.Instance.PlayPooledSound(startReload, 0.8f);
    }

    protected void EvaluateReloadTiming()
    {
        float songPositionInBeats = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPositionInBeats); // Nearest beat
        float timeDifference = Mathf.Abs(songPositionInBeats - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= RhythmDifficulty.perfectThreshold)
        {
            PerfectReload();
            RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
        }
        else if (timeDifference <= RhythmDifficulty.goodThreshold)
        {
            GoodReload();
            RhythmStreakManager.Instance.RegisterHit(streakGainGood);
        }
        else
        {
            BeatUI.Instance.ShowHitFeedback("Miss");
        }
    }

    protected virtual void PerfectReload() { }
    protected virtual void GoodReload() { }
    #endregion
}
