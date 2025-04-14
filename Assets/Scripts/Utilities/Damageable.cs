using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Damageable : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected bool isVulnerable = true;
    public bool isDead = false;
    private bool isBlinking = false; // Prevents multiple calls

    [Header("Visual Effects")]
    [SerializeField] bool blink = true;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem damageEffect;

    [Header("Visual Effects")]
    [SerializeField] GameObject remains;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] float raycastDownDistance = 5f;
    private bool damageEffects = true;

    [Header("Audio")]
    [SerializeField] protected AudioClip[] damageSounds;

    [Header("Debug")]
    [SerializeField] protected float currentHealth;

    protected void Awake()
    {
        currentHealth = maxHealth;
        if (damageEffect)
        damageEffect.gameObject.SetActive(true);

        Initialize();
    }

    protected virtual void Initialize() { }

    public virtual void Heal(float amount)
    {
        if (isDead || amount <= 0)
        return;

        if (currentHealth + amount > maxHealth)
        currentHealth = maxHealth;
        else
        currentHealth += amount;
    }
    public virtual void Damage(float damage)
    {
        if (isDead || !isVulnerable || damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (damageEffects)
        {
            damageEffects = false;

            if (damageEffect)
            damageEffect.Play();
            if (damageSounds.Length > 0)
            AudioManager.Instance.PlayRandomSound(damageSounds, 0.6f, transform);

            Invoke("DamageEfectsCooldown", 0.1f);
        }

        if (blink && !isBlinking)
        {
            if (meshRenderer)
            StartCoroutine(Blink3D());
            if (spriteRenderer)
            StartCoroutine(Blink2D());
        }
    }
    private void DamageEfectsCooldown() { damageEffects = true; }

    public virtual void Die()
    {
        isDead = true;
        SpawnRemains();
        if (damageSounds.Length > 0)
        AudioManager.Instance.PlayRandomSound(damageSounds, 0.5f, transform);
        Destroy(gameObject);
    }
    protected void SpawnRemains()
    {
        if (remains == null) return;

        Vector3 rayOrigin = transform.position;
        RaycastHit hit;

        Vector3 spawnPos;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDownDistance, floorLayer))
        {
            spawnPos = hit.point + Vector3.up * 0.1f;
        }
        else
        {
            // Default to current position if nothing is hit
            spawnPos = transform.position + Vector3.down * 1f;
        }

        Quaternion spawnRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        Instantiate(remains, spawnPos, spawnRot);
    }

    #region Blink

    // 3D Blink Effect
    private IEnumerator Blink3D()
    {
        if (meshRenderer == null || !meshRenderer.material.HasProperty("_Color"))
        yield break;

        isBlinking = true; // Prevents multiple calls

        Material mat = meshRenderer.material;
        Color originalColor = mat.color;
        Color blinkColor = Color.white * 2f;

        float blinkDuration = 0.25f;
        float lerpTime = 0f;
        mat.color = blinkColor;

        while (lerpTime < blinkDuration)
        {
            lerpTime += Time.deltaTime;
            mat.color = Color.Lerp(blinkColor, originalColor, lerpTime / blinkDuration);
            yield return null;
        }

        mat.color = originalColor; // Ensure color is right
        isBlinking = false; // Allows new calls
    }

    // 2D Blink Effect
    private IEnumerator Blink2D()
    {
        if (spriteRenderer == null)
        yield break;

        isBlinking = true; // Prevents multiple calls

        Color originalColor = spriteRenderer.color;
        Color blinkColor = Color.white * 2f;

        float blinkDuration = 0.25f;
        float lerpTime = 0f;
        spriteRenderer.color = blinkColor;

        while (lerpTime < blinkDuration)
        {
            lerpTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(blinkColor, originalColor, lerpTime / blinkDuration);
            yield return null;
        }

        spriteRenderer.color = originalColor; // Ensure color is right
        isBlinking = false; // Allows new calls
    }

    #endregion

}