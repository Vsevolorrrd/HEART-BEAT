using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AIProjectile : MonoBehaviour
{
    //these values are set by the weapon shooting projectile
    [SerializeField] GameObject impactEffect;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;

    public virtual void Initialize(float damage, float speed, float duration)
    {
        this.damage = damage;
        this.speed = speed;
        Destroy(gameObject, duration);
    }

    public void Update()
    {
        MoveUpdate();
    }

    //Inheriting classes can override this to move in a different way
    protected void MoveUpdate()
    {
        float moveby = speed * Time.deltaTime;
        transform.position += transform.forward * moveby;
    }

    private void OnTriggerEnter(Collider collision)
    {
        var targetable = collision.GetComponent<Damageable>();

        Impact(collision);
        if (targetable == null)
        return;

        targetable.Damage(damage);
    }

    //Inheriting classes can override this to have different
    //impact behaviors (such as bouncing on walls, or piercing through enemies)
    public void Impact(Collider collision)
    {
        if (impactEffect)
        Instantiate(impactEffect, transform.position, transform.rotation);

        enabled = false;
        Destroy(gameObject);
    }
}
