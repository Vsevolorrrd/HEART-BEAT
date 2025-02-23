using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Damageable : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] bool isVulnerable = true;
    private bool isDead = false;

    [Header("Visual Effects")]
    public GameObject Remains;

    protected void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHealth = maxHealth;
    }

    public virtual void Damage(float damage)
    {
        if (isDead || !isVulnerable || damage <= 0)
        return;

        Debug.Log("damage");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0)
        return;

        if (currentHealth + amount > maxHealth)
        currentHealth = maxHealth;
        else
        currentHealth += amount;

        //play SFX
    }

    public virtual void Die()
    {
        isDead = true;
        if (Remains != null)
        Instantiate(Remains, gameObject.transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    /*
    protected IEnumerator Flicker()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        m_SpriteRenderer.color = m_OriginalColor;
    }
    */
}
