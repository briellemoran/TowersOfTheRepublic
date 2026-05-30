using UnityEngine;
using UnityEngine.AI;

public class ARCSoldier : MonoBehaviour
{
    [Header("Stats")]
    public float meleeDamage = 20f;
    public float bulletDamage = 15f;
    public float meleeRange = 1.5f;
    public float shootRange = 3f;
    public float attackRate = 1f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LayerMask enemyLayer;

    private NavMeshAgent agent;
    private Vector3 anchorPoint;
    private float patrolRadius = 2.5f;
    private EnemyHealth target;
    private float attackTimer;
    private float patrolTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Vector3 towerPos)
    {
        anchorPoint = towerPos;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        FindTarget();

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist <= meleeRange)
            {
                agent.isStopped = true;
                if (attackTimer >= attackRate)
                {
                    MeleeAttack();
                }
            }
            else if (dist <= shootRange)
            {
                agent.SetDestination(target.transform.position);
                agent.isStopped = false;
                if (attackTimer >= attackRate)
                {
                    ShootAttack();
                }
            }
            else
            {
                // Target is too far, but inside patrol radius?
                agent.SetDestination(target.transform.position);
                agent.isStopped = false;
            }
        }
        else
        {
            Patrol();
        }
    }

    void FindTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(anchorPoint, patrolRadius, enemyLayer);
        float nearestDist = float.MaxValue;
        EnemyHealth nearestEnemy = null;

        foreach (var col in enemies)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestEnemy = col.GetComponent<EnemyHealth>();
            }
        }

        target = nearestEnemy;
    }

    void MeleeAttack()
    {
        target.TakeDamage(meleeDamage);
        attackTimer = 0f;
    }

    void ShootAttack()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(target.transform.position - firePoint.position));
        StandardBullet bullet = bulletGO.GetComponent<StandardBullet>();
        if (bullet != null) bullet.Init(target, bulletDamage);
        attackTimer = 0f;
    }

    void Patrol()
    {
        agent.isStopped = false;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= 2f)
            {
                Vector3 randomPoint = anchorPoint + Random.insideUnitSphere * patrolRadius;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                patrolTimer = 0f;
            }
        }
    }

    public void OnDeath()
    {
        // Simple death handling
        gameObject.SetActive(false);
    }
}
