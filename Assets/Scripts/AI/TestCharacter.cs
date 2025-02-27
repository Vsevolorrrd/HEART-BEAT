using UnityEngine;

public class TestCharacter : Damageable // Just for test
{
    private EmotionController emoController;
    [SerializeField] Transform target;

    private void Update()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 7f);
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
        emoController = GetComponent<EmotionController>();
    }
    public override void Damage(float damage)
    {
        if (isDead)
        return;

        base.Damage(damage);
        if (currentHealth <= 0)
        return;

        emoController.SetEmotion(EmotionType.Hurt);
    }
    public override void Die()
    {
        isDead = true;
        emoController.SetEmotion(EmotionType.DeadInside, false);
    }
}
