using UnityEngine;

public class DistantWeapon : RhythmInput
{
    private Animator anim;

    [Header("Audio")]
    [SerializeField] AudioClip audioclip;
    [SerializeField] AudioClip perfectShot;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip perfectReloadSound;

    [Header("Stats")]
    [SerializeField] Camera cam;
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 100f;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float spread = 0f;
    [SerializeField] LayerMask layerMaskToHit;
    [SerializeField] int maxAmmo = 6;
    private int currentAmmo;

    [Header("Visual Effects")]
    [SerializeField] ParticleSystem GunSmoke;
    [SerializeField] ParticleSystem MuzleFlash;
    [SerializeField] Transform shellEjectionPoint;
    [SerializeField] GameObject shellPrefab;
    [SerializeField] GameObject Impact;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform gunpoint;

    [Header("Camera Shake")]
    [SerializeField] bool camerShake = true;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float shakeAmplitude = 1f;
    [SerializeField] float shakeFrequency = 1f;

    [Header("Reload")]
    [SerializeField] KeyCode reloadKey = KeyCode.R;
    private bool isReloading = false;

    [Header("Debug")] //these are shown in the inspector, but cannot be modified while the game is not running
    [SerializeField] protected float nextShotMinTime = 0; //when can the next attack be fired

    protected override void Start()
    {
        MainMenu.OnPause += HandlePause;
        anim = GetComponent<Animator>();
        currentAmmo = maxAmmo;
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
    }
    protected override void Update()
    {
        if (!playerInput || isBlocked)
        return;

        if (Input.GetKeyDown(reloadKey))
        {
            if (isReloading)
            EvaluateReloadTiming();
            else
            StartReload();
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

        // Reset count if outside press window
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

        Shoot();
    }
    private void Shoot()
    {
        if (currentAmmo <= 0) return;
        currentAmmo--; 
        WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);

        AudioManager.Instance.PlayPooledSound(audioclip, 0.9f);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            float z = Random.Range(-spread, spread);

            // Calculate the direction considering spread
            Vector3 spreadDirection = cam.transform.forward + new Vector3(x, y, z);

            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, spreadDirection, out hit, range, layerMaskToHit))
            {
                Debug.Log(hit.transform.name);

                Damageable target = hit.transform.GetComponent<Damageable>();
                if (target)
                {
                    if (target.isDead == false)
                    EvaluateTimingTarget(target);
                }

                // Calculate direction from hit point towards the camera
                Vector3 direction = (cam.transform.position - hit.point).normalized;
                // Offset the impact towards the camera
                Vector3 impactPosition = hit.point + direction * 0.5f;

                Instantiate(Impact, impactPosition, Quaternion.identity);
            }

        }

        if (camerShake)
        CameraShake.Instance.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
        if (anim != null)
        anim.SetTrigger("Shot");
        if (MuzleFlash != null)
        MuzleFlash.Play();
        if (GunSmoke != null)
        GunSmoke.Play();

        if (currentAmmo <= 0)
        StartReload();
    }
    private void EvaluateTimingTarget(Damageable target)
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");
            RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
            OnPerfectShot(target);
        }
        else if (timeDifference <= goodThreshold)
        {
            HitEffect.Instance.playHitEffect("Good");
            BeatUI.Instance.ShowHitFeedback("Good");
            RhythmStreakManager.Instance.RegisterHit(streakGainGood);
            OnGoodShot(target);
        }
        else
        {
            BeatUI.Instance.ShowHitFeedback("Miss");
            OnMissShot(target);
        }
    }
    private void OnPerfectShot(Damageable target)
    {
        AudioManager.Instance.PlayPooledSound(perfectShot, 0.4f);
        target.Damage(damage * 1.2f);
    }
    private void OnGoodShot(Damageable target)
    {
        target.Damage(damage);
    }
    private void OnMissShot(Damageable target)
    {
        target.Damage(damage * 0.5f);
    }

    #region Reload

    private void StartReload()
    {
        isReloading = true;
        anim.SetBool("StartReload", true);
        WeaponUI.Instance.ShowReloadUI();
    }

    private void EvaluateReloadTiming()
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");

            currentAmmo = maxAmmo;
            isReloading = false;
            WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
            WeaponUI.Instance.HideReloadUI();

            anim.SetBool("StartReload", false);
            anim.SetTrigger("FinishReload");

            AudioManager.Instance.PlayPooledSound(perfectReloadSound, 0.6f);
            AudioManager.Instance.PlayPooledSound(reloadSound, 0.4f);
        }
        else if (timeDifference <= goodThreshold)
        {
            HitEffect.Instance.playHitEffect("Good");
            BeatUI.Instance.ShowHitFeedback("Good");

            currentAmmo = maxAmmo - 1;
            isReloading = false;
            WeaponUI.Instance.UpdateWeaponUI(maxAmmo, currentAmmo);
            WeaponUI.Instance.HideReloadUI();

            anim.SetBool("StartReload", false);
            anim.SetTrigger("FinishReload");

            AudioManager.Instance.PlayPooledSound(reloadSound, 0.4f);
        }
        else
        {
            BeatUI.Instance.ShowHitFeedback("Miss");
        }
    }

    #endregion
}