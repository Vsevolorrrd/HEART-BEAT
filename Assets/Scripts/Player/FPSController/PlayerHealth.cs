using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [SerializeField] float damageCooldown = 0.1f;
    private bool recentlyDamaged = false;

    [Header("Health Bar")]
    [SerializeField] bool useHealthBar = true;
    [SerializeField] Image healthBarBG;
    [SerializeField] Image healthBar;

    [Header("Damage Overlay")]
    [SerializeField] bool useDamageOverlay = true;
    [SerializeField] float DamageOverlayDuration = 1f;
    [SerializeField] CanvasGroup DamageOverlayCG;

    [SerializeField] bool showOverlay;

    [Header("Death screen")]
    [SerializeField] GameObject DeathScreen;

    protected override void Initialize()
    {
        base.Initialize();

        #region Damage Overlay

        if (useDamageOverlay)
        {
            DamageOverlayCG.gameObject.SetActive(true);

            DamageOverlayCG.alpha = 0;
        }
        else
        {
            if (DamageOverlayCG != null)
                DamageOverlayCG.gameObject.SetActive(false);
        }

        #endregion

        #region Health Bar

        if (useHealthBar)
        {
            healthBarBG.gameObject.SetActive(true);
            healthBar.gameObject.SetActive(true);
        }
        else
        {
            healthBarBG.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(false);
        }

        #endregion

        if (DeathScreen != null)
        DeathScreen.SetActive(false);

        showOverlay = false;
    }
    private void Update()
    {
        if (useHealthBar && healthBar != null && !isDead)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
        }
        if (useDamageOverlay && !showOverlay && DamageOverlayCG != null)
        {
            DamageOverlayCG.alpha = Mathf.Max(0, DamageOverlayCG.alpha - 3 * Time.deltaTime);
        }
    }
    public override void Damage(float damage)
    {
        if (recentlyDamaged || isDead || !isVulnerable || damage <= 0)
        return;

        currentHealth -= damage;
        recentlyDamaged = true;

        if (currentHealth <= 0)
        {
            PlayerDeath();
            return;
        }
        else if (useDamageOverlay && DamageOverlayCG != null)
        {
            StartCoroutine(ShowDamageOverlay());
        }
        Invoke(nameof(ResetDamageCooldown), damageCooldown);
    }
    private void ResetDamageCooldown()
    {
        recentlyDamaged = false;
    }
    private IEnumerator ShowDamageOverlay()
    {
        if (DamageOverlayCG == null) yield break;

        DamageOverlayCG.alpha = 1;
        showOverlay = true;

        yield return new WaitForSeconds(DamageOverlayDuration);

        showOverlay = false;
    }
    public override void Heal(float amount)
    {
        if (isDead || amount <= 0)
        return;

        if (currentHealth + amount > maxHealth)
        currentHealth = maxHealth;
        else
        currentHealth += amount;

        //play SFX
    }

    private void PlayerDeath()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true;

        Cursor.lockState = CursorLockMode.None;

        if (DeathScreen)
        DeathScreen.SetActive(true);

        // Disable player object
        gameObject.SetActive(false);
    }
    public void SetVulnerability(bool vulnerable)
    {
        isVulnerable = vulnerable;
    }
}