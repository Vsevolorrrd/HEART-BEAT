using UnityEngine;

public class Subcrusher : AI
{
    [SerializeField] int rechargeBeats = 10;
    [SerializeField] float attackDuration = 1f;
    [SerializeField] GameObject attackCollider;
    [SerializeField] ParticleSystem soundWaveEffect;
    [SerializeField] ParticleSystem chargeEffect;

    private int rechargeCounter = 0;
    private bool isRecharging = false;

    protected override void OnBeat()
    {
        if (isRecharging)
        {
            rechargeCounter++;
            if (rechargeCounter >= rechargeBeats)
            {
                chargeEffect.Play();
                isRecharging = false;
                rechargeCounter = 0;
            }
            return;
        }

        if (currentState == AIState.Fight)
        attack();
    }

    private void attack()
    {
        soundWaveEffect.Play();
        attackCollider.SetActive(true);
        isRecharging = true;
        Invoke("DisableColider", attackDuration);
    }
    private void DisableColider()
    {
        attackCollider.SetActive(false);
    }
}