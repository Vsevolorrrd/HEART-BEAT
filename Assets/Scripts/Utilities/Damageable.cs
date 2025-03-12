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
    [SerializeField] GameObject Remains;

    [Header("Audio")]
    //[SerializeField] AudioClip damageSound;
    //[SerializeField] AudioClip deathSound;

    [Header("Debug")]
    [SerializeField] protected float currentHealth;

    protected void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHealth = maxHealth;
        if (damageEffect)
        damageEffect.gameObject.SetActive(true);
    }

    protected virtual void Heal(float amount)
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

        if (damageEffect)
        damageEffect.Play();

        if (blink && !isBlinking)
        {
            if (meshRenderer)
            StartCoroutine(Blink3D());
            if (spriteRenderer)
            StartCoroutine(Blink2D());
        }
    }

    public virtual void Die()
    {
        isDead = true;
        if (Remains)
        Instantiate(Remains, gameObject.transform.position, Quaternion.identity);
        gameObject.SetActive(false);
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