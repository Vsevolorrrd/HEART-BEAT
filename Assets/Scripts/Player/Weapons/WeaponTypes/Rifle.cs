using UnityEngine;
using System.Collections;

public class Rifle : DistantWeapon
{
    [SerializeField] float shotDelayTime = 1.0f;
    protected override void Shoot()
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;

        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, GetSpreadDirection(), out hit, range, layerMaskToHit))
        {
            Damageable target = hit.transform.GetComponent<Damageable>();
            if (target && !target.isDead)
            EvaluateTimingTarget(target);

            Vector3 direction = (cam.transform.position - hit.point).normalized;
            Vector3 impactPosition = hit.point + direction * 0.5f;

            GetImpactEffect(impactPosition, Quaternion.identity);
        }

        StartCoroutine(FireBulletsWithDelay());

        if (currentAmmo <= 0) StartReload();
    }
    private IEnumerator FireBulletsWithDelay()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            AudioManager.Instance.PlayPooledSound(shotSound, 1f);

            Effects();

            // Wait before firing the next bullet
            yield return new WaitForSeconds(shotDelayTime);
        }
    }
    protected override void OnPerfectShot(Damageable target)
    {
        HitEffect.Instance.playHitEffect("Perfect");
        BeatUI.Instance.ShowHitFeedback("Perfect");
        RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
        AudioManager.Instance.PlayPooledSound(perfectShot, 0.4f);
        target.Damage(damage * 1.5f);
    }

    protected override void OnGoodShot(Damageable target)
    {
        HitEffect.Instance.playHitEffect("Good");
        BeatUI.Instance.ShowHitFeedback("Good");
        RhythmStreakManager.Instance.RegisterHit(streakGainGood);
        target.Damage(damage);
    }
    protected override void OnMissShot(Damageable target)
    {
        BeatUI.Instance.ShowHitFeedback("Miss");
        target.Damage(damage * 0.5f);
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

        currentAmmo = maxAmmo;
        isReloading = false;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
        WeaponUI.Instance.HideReloadUI();

        anim.SetBool("StartReload", false);
        anim.SetTrigger("FinishReload");
    }

    #endregion
}