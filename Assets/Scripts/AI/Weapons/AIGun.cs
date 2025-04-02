using UnityEngine;

public class AIGun : AIWeapon
{
    [Header("Gun Stats")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int projectilesPerShot = 1;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float projectileLifetime = 10f;
    [SerializeField] float projectileSpread = 0f;

    public override void WeaponAttack()
    {
        for (int i = 0; i < projectilesPerShot; i++)
        {
            var go = Instantiate(projectilePrefab, attackPoint.position, GetProjectileSpreadHorizontal(projectileSpread));
            var proj = go.GetComponent<AIProjectile>();
            InitializeProjectile(proj);
        }
    }
    protected virtual void InitializeProjectile(AIProjectile projectile)
    {
        projectile.Initialize(user, attackDamage, projectileSpeed, projectileLifetime);
    }

    protected Quaternion GetProjectileSpreadHorizontal(float currentAccuracy)
    {
        float spreadY = Random.Range(-currentAccuracy, currentAccuracy); // Horizontal spread

        // Apply the spread to the spawnpoint's forward direction
        return Quaternion.Euler(
            attackPoint.rotation.eulerAngles.x,
            attackPoint.rotation.eulerAngles.y + spreadY,
            attackPoint.rotation.eulerAngles.z
        );
    }
}