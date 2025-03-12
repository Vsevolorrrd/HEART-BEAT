using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [Header("Health Bar")]
    public bool useHealthBar = true;
    public Image healthBarBG;
    public Image healthBar;

    [Header("Damage Overlay")]
    public bool useDamageOverlay = true;
    public float DamageOverlayDuration = 1f;
    public CanvasGroup DamageOverlayCG;

    private bool showOverlay;

    [Header("Death screen")]
    public GameObject DeathScreen;

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
        // Handles HealthBar 
        if (useHealthBar && healthBar != null && !isDead)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
        }
        if (useDamageOverlay && !showOverlay)// to hide the bar
        {
            DamageOverlayCG.alpha -= 3 * Time.deltaTime;
        }
    }
    public override void Damage(float damage)
    {
        if (isDead || !isVulnerable || damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
        else
        {
            if (useDamageOverlay)// to show the bar
            {
                StartCoroutine(ShowDamageOverlay());
            }
        }
    }
    private IEnumerator ShowDamageOverlay()
    {
        DamageOverlayCG.alpha = 1;
        showOverlay = true;

        yield return new WaitForSeconds(DamageOverlayDuration);

        showOverlay = false;
    }
    protected override void Heal(float amount)
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
}