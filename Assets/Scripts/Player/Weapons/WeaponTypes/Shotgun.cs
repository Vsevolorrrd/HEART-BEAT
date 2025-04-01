using UnityEngine;

public class Shotgun : DistantWeapon
{
    [SerializeField] AudioClip pumpSound;
    private bool needsPump = false;

    protected override void Update()
    {
        if (!playerInput || isBlocked)
            return;

        if (Input.GetKeyDown(reloadKey))
        {
            if (currentAmmo < maxAmmo)
            {
                if (isReloading)
                EvaluateReloadTiming();
                else
                StartReload();
            }
        }

        if (isReloading) return;

        if (Input.GetKeyDown(actionKey))
        {
            if (currentAmmo > 0)
            HandleKeyPress();
            else
            StartReload();
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

        if (needsPump)
        {
            PumpShotgun();
            return;
        }

        Shoot();
    }

    protected override void Shoot()
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);

        AudioManager.Instance.PlayPooledSound(shotSound, 0.9f);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, GetSpreadDirection(), out hit, range, layerMaskToHit))
            {
                Debug.Log(hit.transform.name);

                Damageable target = hit.transform.GetComponent<Damageable>();
                if (target && !target.isDead)
                EvaluateTimingTarget(target);

                Vector3 direction = (cam.transform.position - hit.point).normalized;
                Vector3 impactPosition = hit.point + direction * 0.5f;

                Instantiate(impact, impactPosition, Quaternion.identity);
            }
        }

        Effects();
        needsPump = true;
    }
    private void PumpShotgun()
    {
        float songPositionInBeats = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPositionInBeats);
        float timeDifference = Mathf.Abs(songPositionInBeats - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= RhythmDifficulty.perfectThreshold + thresholdModifier)
        {
            OnPerfectPump();
        }
        else if (timeDifference <= RhythmDifficulty.goodThreshold + thresholdModifier)
        {
            OnGoodPump();
        }
        else
        {
            OnMissPump();
        }
    }
    private void OnPerfectPump()
    {
        AudioManager.Instance.PlayPooledSound(pumpSound, 1f);
        RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
        HitEffect.Instance.playHitEffect("Perfect");
        BeatUI.Instance.ShowHitFeedback("Perfect");

        needsPump = false;
        anim.SetTrigger("PumpShotgun");
    }

    private void OnGoodPump()
    {
        RhythmStreakManager.Instance.RegisterHit(streakGainGood);
        AudioManager.Instance.PlayPooledSound(pumpSound, 1f);
        HitEffect.Instance.playHitEffect("Good");
        BeatUI.Instance.ShowHitFeedback("Good");

        needsPump = false;
        anim.SetTrigger("PumpShotgun");
    }

    private void OnMissPump()
    {
        BeatUI.Instance.ShowHitFeedback("Miss");
    }

    #region Reload

    protected override void PerfectReload()
    {
        AudioManager.Instance.PlayPooledSound(perfectReload, 0.5f);
        AudioManager.Instance.PlayPooledSound(reload, 1f);

        HitEffect.Instance.playHitEffect("Perfect");
        BeatUI.Instance.ShowHitFeedback("Perfect");

        currentAmmo = maxAmmo;
        isReloading = false;
        needsPump = false;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
        WeaponUI.Instance.HideReloadUI();

        anim.SetBool("StartReload", false);
        anim.SetTrigger("FinishReload");
    }
    protected override void GoodReload()
    {
        AudioManager.Instance.PlayPooledSound(reload, 1f);

        HitEffect.Instance.playHitEffect("Good");
        BeatUI.Instance.ShowHitFeedback("Good");

        currentAmmo = maxAmmo - 1;
        isReloading = false;
        needsPump = false;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
        WeaponUI.Instance.HideReloadUI();

        anim.SetBool("StartReload", false);
        anim.SetTrigger("FinishReload");
    }

    #endregion
}