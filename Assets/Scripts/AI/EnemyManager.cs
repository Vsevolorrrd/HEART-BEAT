using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] int heavyAttacksMax = 1;
    private int currrentHeavyAttacks = 0;
    
    public bool RequestHeavyAttack()
    {
        if (currrentHeavyAttacks < heavyAttacksMax)
        {
            currrentHeavyAttacks++;
            return true;
        }

        return false;
    }
    public void FinishedHeavyAttack()
    {
        currrentHeavyAttacks--;
    }
}
