using System.Collections;
using UnityEngine;

public class Beatling : AI
{
    private EmotionController emoController;

    [Header("Scribbles")]
    [SerializeField] GameObject[] scribbles;

    [Header("Attack")]
    [SerializeField] int attackDelayInBeats = 2;
    [SerializeField] AIWeapon weapon;
    [SerializeField] bool lungeAttack;
    private float lungeDuration = 0.5f;

    private int beatsUntilAttack = 0;

    protected override void Initialize()
    {
        base.Initialize();
        emoController = GetComponent<EmotionController>();
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
                beatsUntilAttack = attackDelayInBeats; // Reset delay
                if (lungeAttack)
                StartCoroutine(LungeAttack());
                else
                weapon.WeaponAttack();
            }
        }
        else
        {
            beatsUntilAttack = 0;
        }
    }
    private IEnumerator LungeAttack()
    {
        weapon.WeaponAttack();
        agent.speed *= 3; // Increase speed during lunge
        float elapsedTime = 0f;

        while (elapsedTime < lungeDuration)
        {
            if (target != null)
                agent.SetDestination(target.position); // Update position during lunge

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        agent.speed = speed;
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

        for (int i = 0; i < scribbles.Length; i++)
        {
            scribbles[i].gameObject.SetActive(false);
        }
        if(ArenaSpawn)
        ArenaSpawn.Died();
    }
}