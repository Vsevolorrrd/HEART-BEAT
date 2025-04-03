using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageColider : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damageable target))
        {
            target.Damage(damage);
        }
    }
}
