using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float detectionRange = 4.0f;
    public float attackRange = 2.0f;
    public float damage = 10f;
    public float attackRate = 1.0f;
    
    private float attackTimer;
    private SoldierHealth target;
    private EnemyPathFollower pathFollower;

    void Start()
    {
        pathFollower = GetComponent<EnemyPathFollower>();
        attackTimer = attackRate;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindTarget();
        }

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            
            // Look at target
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
            }

            if (dist <= attackRange)
            {
                // Stop moving to attack
                if (pathFollower != null && pathFollower.enabled) 
                {
                    pathFollower.enabled = false;
                }
                
                if (attackTimer >= attackRate)
                {
                    Attack();
                }
            }
            else if (dist <= detectionRange)
            {
                if (pathFollower != null && !pathFollower.enabled) pathFollower.enabled = true;
            }
            else
            {
                // Target moved too far
                target = null;
                if (pathFollower != null && !pathFollower.enabled) pathFollower.enabled = true;
            }
        }
        else
        {
            if (pathFollower != null && !pathFollower.enabled)
            {
                pathFollower.enabled = true;
            }
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        float closestDist = float.MaxValue;
        SoldierHealth closestSoldier = null;

        foreach (var hit in hits)
        {
            SoldierHealth health = hit.GetComponent<SoldierHealth>();
            if (health != null)
            {
                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    closestSoldier = health;
                }
            }
        }
        target = closestSoldier;
    }

    void Attack()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
            attackTimer = 0f;
        }
    }
}
