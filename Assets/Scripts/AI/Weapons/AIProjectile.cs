using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AIProjectile : MonoBehaviour
{
    //these values are set by the weapon shooting projectile
    [SerializeField] GameObject impactEffect;
    [SerializeField] Damageable owner;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;

    public void Initialize(Damageable owner, float damage, float speed, float duration)
    {
        this.owner = owner;
        this.damage = damage;
        this.speed = speed;
        Destroy(gameObject, duration);
    }

    public void Update()
    {
        MoveUpdate();
    }
    protected void MoveUpdate()
    {
        float moveby = speed * Time.deltaTime;
        transform.position += transform.forward * moveby;
    }

    private void OnTriggerEnter(Collider collision)
    {
        var targetable = collision.GetComponent<Damageable>();

        if (targetable == owner)
        return;

        Impact();

        if (targetable == null)
        return;

        targetable.Damage(damage);
    }
    public void Impact()
    {
        if (impactEffect)
        Instantiate(impactEffect, transform.position, transform.rotation);

        enabled = false;
        Destroy(gameObject);
    }
}