using UnityEngine;
using System.Collections;

public class Metrazer : AI
{
    [SerializeField] int shootDelayBeats = 5;
    [SerializeField] int rechargeBeats = 10;
    [SerializeField] float laserDamage = 50f;
    [SerializeField] float laserRange = 100f;
    [SerializeField] float rotationAngle = 45f;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform metronome;
    [SerializeField] GameObject laserPrefab;

    [Header("Metrazer Sounds")]
    [SerializeField] AudioClip shot;
    [SerializeField] AudioClip[] countdown;

    private int beatCounter = 0;
    private int rechargeCounter = 0;
    private bool isRecharging = false;
    private bool right;
    private bool hint = true;
    private bool attack = false;

    protected override void Initialize()
    {
        base.Initialize();
    }
    protected override void ChangeState(AIState newState)
    {
        base.ChangeState(newState);
        if (newState != AIState.Fight)
        {
            if (!attack)
            beatCounter = 0;
        }
    }
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
                BeatUI.Instance.AddHintDots("hintDotLaser");
                hint = false;
            }

            Debug.Log(beatCounter);

            // Play countdown sound if not exceeded
            if (beatCounter < countdown.Length)
            {
                AudioManager.Instance.PlaySound(countdown[beatCounter], 1f);
            }

            beatCounter++;
            RotateMetronome();

            if (beatCounter >= shootDelayBeats)
            {
                Shoot();
                ResetMetronome();
                beatCounter = 0;
                isRecharging = true;
                attack = false;
                hint = true;
            }
        }
    }
    private void RotateMetronome()
    {
        if (right)
        {
            metronome.rotation = Quaternion.Euler(0, 0, rotationAngle);
            right = false;
        }
        else
        {
            metronome.rotation = Quaternion.Euler(0, 0, -rotationAngle);
            right = true;
        }
    }
    private void ResetMetronome()
    {
        metronome.rotation = Quaternion.Euler(0, 0, 0);
        right = true;
    }

    private void Shoot()
    {
        if (target == null) return;

        AudioManager.Instance.PlaySound(shot, 0.7f, transform);
        Instantiate(laserPrefab, attackPoint.position, Quaternion.Euler(attackPoint.rotation.eulerAngles));

        Vector3 shootDirection = (target.position - attackPoint.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(attackPoint.position, shootDirection, out hit, laserRange))
        {

            Damageable targetHit = hit.collider.GetComponent<Damageable>();
            if (targetHit != null)
            {
                StartCoroutine(DelayedDamage(targetHit, 0.05f));
            }
        }

        EnemyManager.Instance.FinishedHeavyAttack();
    }
    private IEnumerator DelayedDamage(Damageable targetHit, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetHit != null)
        {
            targetHit.Damage(laserDamage);
        }
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