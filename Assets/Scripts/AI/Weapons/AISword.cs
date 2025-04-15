using UnityEngine;

public class AISword : AIWeapon
{
    [SerializeField] float attackRadious = 2f;
    [SerializeField] AudioClip attackClip;

    protected Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public override void WeaponAttack()
    {
        if (anim)
        {
            AudioManager.Instance.PlaySound(attackClip, 1f, transform);
            anim.SetTrigger("Attack");
            return;
        }

        AISwordAttack();
    }

    public void AISwordAttack() // called through animation
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadious, damageableLayerMask);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject == user.gameObject) continue;// dont damage yourself
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
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadious);
        }
    }
}