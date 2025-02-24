using UnityEngine;

public class MeleeWeapon : RhythmInput
{
    [Header("MeleeWeapon")]
    [SerializeField] float damage = 10f;
    [SerializeField] float heavyDamage = 15f;
    [SerializeField] float heavyReadyTime = 1f;
    [SerializeField] float comboTime = 1f;


    private float timeSinceLastAttack;
    private float heavyPrepTime;
    private float currentDamage;
    private int currentAttack;
    private bool heavy = false;

    public override void Start()
    {
        base.Start();
        currentAttack = 0;
        currentDamage = damage;
    }
    public override void Update()
    {
        if (Input.GetKey(actionKey))
        {
            heavyPrepTime += Time.deltaTime;
            if (heavyPrepTime > 0.5)
            {
                heavyAttackPrep();
            }
        }
        if (Input.GetKeyUp(actionKey))
        {
            EvaluateTiming();
        }
    }
    public void Attack()
    {
        currentDamage = damage;
        currentAttack++;
        if (currentAttack > 2)
        {
            currentAttack = 1;
        }
        if (timeSinceLastAttack > comboTime)
        {
            currentAttack = 1;
        }
        //Anim.SetTrigger("Attack" + currentAttack);

        timeSinceLastAttack = 0;
    }
    public void heavyAttackPrep()
    {
        //Anim.SetBool("HeavyAttackPreporation", true);
        if (heavyPrepTime >= heavyReadyTime)
        {
            heavy = true;
        }
    }
    public void heavyAttack()
    {
        //Anim.SetTrigger("HeavyAttack");
        currentDamage = heavyDamage;
        heavy = false;
    }

    public override void OnPerfectHit()
    {
        if (heavy)
        {
            heavyAttack();
            heavyPrepTime = 0;
        }
        else
        {
            Attack();
            heavyPrepTime = 0;
        }
    }
    public override void OnGoodHit()
    {
        if (heavy)
        {
            heavyAttack();
            heavyPrepTime = 0;
        }
        else
        {
            Attack();
            heavyPrepTime = 0;
        }
    }
}
