using UnityEngine;

public class MeleeWeapon : RhythmInput
{
    [Header("Melee Attack")]
    [SerializeField] float damage = 50f;
    [SerializeField] float attackCooldown = 0.2f;
    [SerializeField] float attackRadious = 3f;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask attackLayer;
    private float lastAttackTime = 0f;

    [Header("Combo System")]
    [SerializeField] float comboResetTime = 1f;
    [SerializeField] float comboMultiplier = 1.5f;
    private bool lastAttackUp = false;
    private bool comboActive = false;
    private float lastComboTime = 0f;

    [Header("Scroll Sensitivity")]
    [SerializeField] float scrollThreshold = 0.2f;
    private float scroll = 0f;

    private Animator anim;

    protected override void Start()
    {
        MainMenu.OnPause += HandlePause;
        anim = GetComponent<Animator>();
    }
    protected override void Update()
    {
        if (!playerInput || isBlocked)
            return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0) // If any scroll input is detected
        {
            scroll += scrollInput;

            if (Mathf.Abs(scroll) >= scrollThreshold) // If threshold is exceeded
            {
                if (scroll > 0)
                {
                    HandleScrollAttack(true);
                }
                else if (scroll < 0)
                {
                    HandleScrollAttack(false);
                }
                scroll = 0f; // Reset after registering an attack
            }
        }
    }

    private void HandleScrollAttack(bool isUp)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        return; // Prevent spam

        if (Time.time - lastComboTime <= comboResetTime && lastAttackUp != isUp)
        {
            comboActive = true;
        }
        else
        {
            comboActive = false;
        }

        lastAttackUp = isUp;
        lastComboTime = Time.time;

        EvaluateTiming();
        lastAttackTime = Time.time;
    }

    protected override void OnPerfectHit()
    {
        ExecuteAttack(2f, 2f);
    }

    protected override void OnGoodHit()
    {
        ExecuteAttack(1.5f, 1.5f);
    }

    protected override void OnMiss()
    {
        ExecuteAttack(1f, 1f);
    }

    private void ExecuteAttack(float damageModifier, float radiousModifier)
    {
        float damageOverall = damage * damageModifier;
        float radiousOverall = attackRadious * radiousModifier;

        if (comboActive)
        {
            damageOverall = damageOverall * comboMultiplier;
        }


        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, radiousOverall, attackLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Damageable target = enemy.GetComponent<Damageable>();
            if (target != null)
            {
                target.Damage(damageOverall);
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }
            }
        }

        if (lastAttackUp)
        {
            if (anim)
            anim.SetTrigger("AttackUp");
        }
        else
        {
            if (anim)
            anim.SetTrigger("AttackDown");
        }
    }
}