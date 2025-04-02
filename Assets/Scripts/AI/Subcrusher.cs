using UnityEngine;

public class Subcrusher : AI
{
    [SerializeField] int rechargeBeats = 10;
    [SerializeField] float attackDuration;
    [SerializeField] GameObject attackCollider;

    private int rechargeCounter = 0;
    private bool isRecharging = false;

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
        attack();
    }

    private void attack()
    {
        attackCollider.SetActive(true);
        isRecharging = true;
        Invoke("DisableColider", attackDuration);
    }
    private void DisableColider()
    {
        attackCollider.SetActive(false);
    }

}
