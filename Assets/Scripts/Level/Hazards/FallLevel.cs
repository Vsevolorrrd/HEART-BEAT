using UnityEngine;

public class FallLevel : MonoBehaviour
{
    private float damage = 9999f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DeathScreen.Instance.FallDeath();
            return;
        }
        if (other.TryGetComponent(out Damageable target))
        {
            target.Damage(damage);
        }
    }
}