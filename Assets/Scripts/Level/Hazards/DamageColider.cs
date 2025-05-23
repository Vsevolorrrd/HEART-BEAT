using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DamageColider : MonoBehaviour
{
    public float damage;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Damageable target))
        {
            target.Damage(damage);
        }
    }
}