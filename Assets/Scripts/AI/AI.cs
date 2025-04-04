using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Actor))]
public class AI : Damageable
{
    public enum AIState
    {
        Idle,
        Chase,
        Fight,
        Stunned
    }

    [Header("AI")]
    public AIState currentState;

    public ArenaSpawn ArenaSpawn;
    protected NavMeshAgent agent;
    protected Actor actor;
    protected bool hasAlerted = false;

    [Header("Basic Behaviour")]
    [SerializeField] protected float sightRange = 75f;
    [SerializeField] protected float attackRange = 3f;
    [SerializeField] protected float retreatRange = 20f;
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected bool canRetreat = false;
    private float checkInterval = 3f;
    private float stunTimer = 0f;


    [Header("Debug")]
    [SerializeField] protected int currentPatrolPoint = 0;
    [SerializeField] protected Transform target;


    //[Header("Sound Effects")]


    protected override void Initialize()
    {
        base.Initialize();

        BEAT_Manager.BEAT += OnBeat;
        agent = GetComponent<NavMeshAgent>();
        actor = GetComponent<Actor>();
        agent.speed = speed;
        currentState = AIState.Idle;
        checkInterval = checkInterval + Random.Range(-1f, 1f); // so the AI dont update on the same frames

        StartCoroutine(RecheckTarget()); // Start periodic enemy rechecking
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (target == null) target = FindEnemy()?.transform;
        if (target == null) return;

        switch (currentState)
        {
            case AIState.Idle:
                Idle();
                if (CanSeeTarget()) ChangeState(AIState.Chase);
                break;

            case AIState.Chase:
                ChaseTarget();
                if (IsInAttackRange()) ChangeState(AIState.Fight);
                break;

            case AIState.Fight:
                AttackTarget();
                if (!IsInAttackRange()) ChangeState(AIState.Chase);
                break;

            case AIState.Stunned:
                Stunned();
                break;
        }
    }

    protected virtual void ChangeState(AIState newState)
    {
        agent.isStopped = false;
        currentState = newState;
    }
    protected Actor FindEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        Actor nearestEnemy = null;
        float minSqrDistance = sightRange * sightRange; // Use squared distance for optimization
        Vector3 myPosition = transform.position;

        foreach (Collider col in colliders)
        {
            Actor act = col.GetComponent<Actor>();
            if (act == null || act == this || act.faction == actor.faction) continue;

            float sqrDistance = (act.transform.position - myPosition).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestEnemy = act;
            }
        }

        return nearestEnemy;
    }

    protected virtual void Idle()
    {
        if (patrolPoints.Length > 0)
        {
            Patrol();
            return;
        }
    }
    protected virtual void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.SetDestination(patrolPoints[currentPatrolPoint].position);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1f)
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
    }
    protected virtual void ChaseTarget()
    {
        agent.SetDestination(target.position);
    }
    protected virtual void AttackTarget()
    {
        // Stop the agent immediately
        agent.isStopped = true;
    }
    protected virtual void Stunned()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            ChangeState(AIState.Idle); // or return to a previous state if you track that
        }
    }
    public void Stun(float duration)
    {
        stunTimer = duration;
        ChangeState(AIState.Stunned);
        agent.isStopped = true;
    }

    // Utility functions
    bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    bool CanSeeTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, sightRange))
        {
            Damageable tar = hit.transform.GetComponent<Damageable>();
            if (tar != null)
            {
                return true;
            }
        }
        return false;
    }
    private IEnumerator RecheckTarget()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(checkInterval); // Adjust check speed dynamically

            Actor newTarget = FindEnemy();
            if (newTarget != null && newTarget.transform != target)
            {
                target = newTarget.transform; // Switch to a better target
            }
        }
    }
    public override void Die()
    {
        base.Die();
        if (ArenaSpawn)
        ArenaSpawn.Died();
    }
    #region events

    public virtual void OnDestroy()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }

    protected virtual void OnBeat() { }


    #endregion
}