using UnityEngine;
using System.Collections;

public class Metrazer : AI
{
    [SerializeField] int shootDelayBeats = 5;
    [SerializeField] int rechargeBeats = 10;
    [SerializeField] float laserDamage = 50f;
    [SerializeField] float laserRange = 100f;
    [SerializeField] float rotationAngle = 45f;
    [SerializeField] LineRenderer laserBeam;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform metronome;

    private int beatCounter = 0;
    private int rechargeCounter = 0;
    private bool isRecharging = false;
    private bool right;

    protected override void Initialize()
    {
        base.Initialize();
        laserBeam.enabled = false;
    }
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
            beatCounter++;
            RotateMetronome();

            if (beatCounter >= shootDelayBeats)
            {
                Shoot();
                ResetMetronome();
                beatCounter = 0;
                isRecharging = true;  // Start recharge after shooting
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

        Vector3 shootDirection = (target.position - attackPoint.position).normalized;
        RaycastHit hit;

        Vector3 hitPosition;

        if (Physics.Raycast(attackPoint.position, shootDirection, out hit, laserRange))
        {
            hitPosition = hit.point;

            Damageable targetHit = hit.collider.GetComponent<Damageable>();
            if (targetHit != null)
            {
                targetHit.Damage(laserDamage);
            }
        }
        else
        {
            hitPosition = attackPoint.position + shootDirection * laserRange;
        }

        StartCoroutine(ShowLaserEffect(hitPosition));
    }

    private IEnumerator ShowLaserEffect(Vector3 hitPoint)
    {
        laserBeam.enabled = true;

        Vector3 localStart = attackPoint.InverseTransformPoint(attackPoint.position);
        Vector3 localEnd = attackPoint.InverseTransformPoint(hitPoint);

        laserBeam.SetPosition(0, localStart);
        laserBeam.SetPosition(1, localEnd);

        yield return new WaitForSeconds(0.5f); // Laser visible for a short time

        laserBeam.enabled = false;
    }
}