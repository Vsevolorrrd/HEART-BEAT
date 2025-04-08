using UnityEngine;

public class Subcrusher : AI
{
    [SerializeField] int attackDelayBeats = 2;
    [SerializeField] int rechargeBeats = 10;
    [SerializeField] float attackDuration = 1f;
    [SerializeField] GameObject attackCollider;
    [SerializeField] ParticleSystem soundWaveEffect;
    [SerializeField] ParticleSystem chargeEffect;

    private int beatCounter = 0;
    private int rechargeCounter = 0;
    private bool isRecharging = false;
    private bool hint = true;
    private bool attack = false;

    protected override void OnBeat()
    {
        if (isDead) return;

        if (isRecharging)
        {
            rechargeCounter++;
            if (rechargeCounter >= rechargeBeats)
            {
                isRecharging = false;
                rechargeCounter = 0;
            }
            return;
        }

        if (currentState == AIState.Fight && EnemyManager.Instance.RequestHeavyAttack())
        attack = true;

        if (attack)
        {
            if (hint)
            {
                BeatUI.Instance.AddHintDots(Color.red);
                hint = false;
            }
            chargeEffect.Play();
            isRecharging = false;
            rechargeCounter = 0;

            beatCounter++;

            if (beatCounter >= attackDelayBeats)
            {
                Attack();
                beatCounter = 0;
            }
        }
    }

    private void Attack()
    {
        soundWaveEffect.Play();
        attackCollider.SetActive(true);
        isRecharging = true;
        hint = true;
        attack = false;
        Invoke("DisableColider", attackDuration);
    }
    private void DisableColider()
    {
        attackCollider.SetActive(false);
        EnemyManager.Instance.FinishedHeavyAttack();
    }
    public override void Die()
    {
        if (attack) // if enemy dies while attacking, he needs to free space for other enemies
        {
            EnemyManager.Instance.FinishedHeavyAttack();
            BeatUI.Instance.RemoveHintDots();
        }
        base.Die();
    }
}