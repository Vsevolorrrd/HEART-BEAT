using UnityEngine;

public class DistantWeapon : MonoBehaviour
{
    private Animator anim;

    [SerializeField] AudioClip audioclip;
    [SerializeField] AudioClip perfectShot;

    [Header("RhythmInput")]
    public float perfectThreshold = 0.1f;  // 100 ms for Perfect
    public float streakGainPerfect = 2f;
    public float goodThreshold = 0.15f;    // 150 ms for Good
    public float streakGainGood = 1f;
    public KeyCode actionKey = KeyCode.Space;
    [HideInInspector] public bool playerInput = true;

    [Header("Stats")]
    [SerializeField] Camera cam;
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    [SerializeField] float cooldown = 0.2f;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float spread = 0f;

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

    [Header("Debug")]
    //these are shown in the inspector, but cannot be modified while the game is not running
    [SerializeField] protected float nextShotMinTime = 0; //when can the next attack be fired

    private void Start()
    {
        MainMenu.OnPause += HandlePause;
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!playerInput)
        return;

        if (Input.GetKeyDown(actionKey))
        {
            if (nextShotMinTime > cooldown)
            return;

            Shoot();
        }
    }
    private void EvaluateTiming(Damageable target)
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");
            RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
            OnPerfectHit(target);
        }
        else if (timeDifference <= goodThreshold)
        {
            HitEffect.Instance.playHitEffect("Good");
            BeatUI.Instance.ShowHitFeedback("Good");
            RhythmStreakManager.Instance.RegisterHit(streakGainGood);
            OnGoodHit(target);
        }
        else
        {
            BeatUI.Instance.ShowHitFeedback("Miss");
            OnMiss(target);
        }
    }
    private void OnPerfectHit(Damageable target)
    {
        AudioManager.Instance.PlayPooledSound(perfectShot);
        Debug.Log("Damaging");
        target.Damage(damage * 1.2f);
    }
    private void OnGoodHit(Damageable target)
    {
        Debug.Log("Damaging");
        target.Damage(damage);
    }
    private void OnMiss(Damageable target)
    {
        Debug.Log("Damaging");
        target.Damage(damage * 0.5f);
    }
    private void Shoot()
    {
        AudioManager.Instance.PlayPooledSound(audioclip);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            float z = Random.Range(-spread, spread);

            // Calculate the direction considering spread
            Vector3 spreadDirection = cam.transform.forward + new Vector3(x, y, z);

            RaycastHit hit;

            if (Physics.Raycast(cam.transform.position, spreadDirection, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Damageable target = hit.transform.GetComponent<Damageable>();
                if (target != null)
                {
                    EvaluateTiming(target);
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

        /*
        if (GunSmoke != null)
        GunSmoke.Play();
        if (muzzleFlashLight != null)
        StartCoroutine(Flash());       
        if (shellEjectionPoint != null && shellPrefab != null)
        EjectShell();
        */
    }

    #region events

    private void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;
    }

    private void HandlePause(bool isPaused)
    {
        playerInput = !isPaused;
    }
    #endregion
}