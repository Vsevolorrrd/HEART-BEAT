using UnityEngine;

public class Beatling : AI
{
    private EmotionController emoController;
    private AIOnDeath aiOnDeath;

    [Header("Scribbles")]
    [SerializeField] GameObject[] scribbles;

    [Header("Melee Attack")]
    [SerializeField] int attackDelayInBeats = 2;
    [SerializeField] AIWeapon weapon;

    private int beatsUntilAttack = 0;

    protected override void Initialize()
    {
        base.Initialize();
        emoController = GetComponent<EmotionController>();
        aiOnDeath = GetComponent<AIOnDeath>();
        weapon.SetUser(this);
    }
    protected override void OnBeat()
    {
        if (isDead) return;

        if (currentState == AIState.Fight)
        {
            if (beatsUntilAttack > 0)
            {
                beatsUntilAttack--; // Countdown until attack
            }
            else
            {
                emoController.SetEmotion(EmotionType.SuperAngry);
                weapon.WeaponAttack();
                beatsUntilAttack = attackDelayInBeats; // Reset delay
            }
        }
        else
        {
            beatsUntilAttack = 0;
        }
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
        agent.isStopped = true;
        emoController.SetEmotion(EmotionType.DeadInside, false);
        weapon.gameObject.SetActive(false);
        if (aiOnDeath)
        aiOnDeath.Dead();

        for (int i = 0; i < scribbles.Length; i++)
        {
            scribbles[i].gameObject.SetActive(false);
        }
    }
}