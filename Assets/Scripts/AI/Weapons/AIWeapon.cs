using UnityEngine;

public abstract class AIWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] protected float attackDamage = 25f;
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask damageableLayerMask;

    protected AI user;
    public void SetUser(AI ai)
    {
        user = ai;
    }

    public virtual void WeaponAttack()
    {

    }
}