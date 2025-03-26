using UnityEngine;

public class MeleeWeapon : RhythmInput
{
    [Header("Melee Weapon")]
    [SerializeField] float damage = 10f;
    [SerializeField] float comboTime = 1f;
    [SerializeField] float attackRadious = 3f;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask attackLayer;

    private float timeSinceLastAttack = 0f;
    private int currentAttack = 0; // Combo counter
    private Animator anim;

    [Header("Debug")] //these are shown in the inspector, but cannot be modified while the game is not running
    [SerializeField] protected float nextAttackMinTime = 0; //when can the next attack be fired

    protected override void Start()
    {
        base.Start();
        currentAttack = 0;
    }
    protected override void Update()
    {
        if (!playerInput || isBlocked)
        return;

        timeSinceLastAttack += Time.deltaTime;

        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }
    }
    private void NormalAttack(float damageModifier, float radiousModifier)
    {
        float damageOverall = damage * damageModifier;
        float radiousOverall = attackRadious * radiousModifier;
        currentAttack++;

        if (currentAttack > 2 || timeSinceLastAttack > comboTime)
        {
            currentAttack = 1;
        }
        if (anim)
        anim.SetTrigger(currentAttack);

        Attack(damageOverall, radiousOverall);
        timeSinceLastAttack = 0;
    }
    public void Attack(float attackDamage, float radious)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, radious, attackLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Damageable target = enemy.GetComponent<Damageable>();
            if (target != null)
            {
                target.Damage(attackDamage);
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }

    protected override void OnPerfectHit()
    {
        NormalAttack(2f, 1.5f);
    }
    protected override void OnGoodHit()
    {
        NormalAttack(1.5f, 1.3f);
    }
    protected override void OnMiss()
    {
        NormalAttack(1f, 1f);
    }
}