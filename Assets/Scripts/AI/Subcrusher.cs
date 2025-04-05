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

    protected override void OnBeat()
    {
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

        if (currentState == AIState.Fight)
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
                attack();
                beatCounter = 0;
                hint = true;
            }
        }
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