using UnityEngine;

public class Revolver : DistantWeapon
{

    protected override void Update()
    {
        if (!PlayerManager.Instance.playerInput || isBlocked)
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

        if (Input.GetKeyDown(actionKey))
        {
            if (isReloading)
            {
                WeaponUI.Instance.HideReloadUI();
                anim.SetBool("StartReload", false);
                isReloading = false;
            }
            if (currentAmmo > 0)
            Shoot();
            else
            StartReload(); // If out of bullets, force reload
        }
    }

    protected override void Shoot()
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);

        AudioManager.Instance.PlayPooledSound(shotSound, 1f);

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

        Effects();
        if (currentAmmo <= 0) StartReload();
    }

    #region Reload

    protected override void PerfectReload()
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo++;
            WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");

            anim.SetTrigger("InsertBullet");

            AudioManager.Instance.PlayPooledSound(perfectReload, 0.5f);
            AudioManager.Instance.PlayPooledSound(reload, 0.7f);

            if (currentAmmo == maxAmmo)
            {
                WeaponUI.Instance.HideReloadUI();
                anim.SetBool("StartReload", false);
                isReloading = false;
            }
        }
    }
    protected override void GoodReload()
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo++;
            WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
            HitEffect.Instance.playHitEffect("Good");
            BeatUI.Instance.ShowHitFeedback("Good");

            anim.SetTrigger("InsertBullet");

            AudioManager.Instance.PlayPooledSound(reload, 0.7f);

            if (currentAmmo == maxAmmo)
            {
                WeaponUI.Instance.HideReloadUI();
                anim.SetBool("StartReload", false);
                isReloading = false;
            }
        }
    }

    #endregion
}