using UnityEngine;

public class ARFTrooperTower : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float rotationSpeed = 30f;
    public float maxRotationAngle = 90f;

    [Header("Attack Settings")]
    public float damage = 25f;
    public float fireRate = 1.2f;
    public float detectionRange = 8f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform turret;
    public LayerMask enemyLayer;

    [Header("General Settings")]
    public float health = 100f;
    public GameObject destroyEffectPrefab;

    private TowerState currentState = TowerState.Patrol;
    private EnemyHealth target;
    private float fireCooldown;
    private bool isTowerDead = false;

    private Vector3 barrelLocalAxis;

    void Start()
    {
        if (turret != null && firePoint != null)
        {
            // Identify which local axis of the turret points towards the firePoint
            barrelLocalAxis = turret.InverseTransformDirection(firePoint.position - turret.position).normalized;
        }
        else
        {
            barrelLocalAxis = Vector3.forward;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case TowerState.Patrol:
                Patrol();
                break;
            case TowerState.Attack:
                Attack();
                break;
            case TowerState.Die:
                Die();
                break;
        }
    }

    void Patrol()
    {
        if (turret != null)
        {
            // Simple left-right ping-pong around the base's up axis
            float angle = Mathf.PingPong(Time.time * rotationSpeed, maxRotationAngle * 2) - maxRotationAngle;
            
            // Calculate a target rotation that is 'angle' degrees away from the base's forward
            Vector3 targetForward = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            RotateTurretTo(targetForward);
        }

        LookForEnemies();
    }

    void LookForEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        EnemyHealth closestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
            currentState = TowerState.Attack;
        }
    }

    void Attack()
    {
        if (target == null || !target.gameObject.activeInHierarchy || Vector3.Distance(transform.position, target.transform.position) > detectionRange)
        {
            target = null;
            currentState = TowerState.Patrol;
            return;
        }

        // Rotate toward target
        Vector3 direction = (target.transform.position - turret.position).normalized;
        RotateTurretTo(direction);

        // Shooting logic
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    void RotateTurretTo(Vector3 targetDir)
    {
        if (turret == null) return;

        // Project both the current visual barrel and the target direction onto the horizontal plane
        Vector3 currentBarrelWorld = turret.TransformDirection(barrelLocalAxis);
        currentBarrelWorld = Vector3.ProjectOnPlane(currentBarrelWorld, transform.up).normalized;

        Vector3 targetDirHorizontal = Vector3.ProjectOnPlane(targetDir, transform.up).normalized;

        if (currentBarrelWorld != Vector3.zero && targetDirHorizontal != Vector3.zero)
        {
            // Find the angle between where we are looking and where we want to look
            float angle = Vector3.SignedAngle(currentBarrelWorld, targetDirHorizontal, transform.up);
            
            // Apply a fraction of that rotation for smoothness
            float step = angle * Time.deltaTime * (rotationSpeed / 10f); // Adjust divisor for feel
            turret.rotation = Quaternion.AngleAxis(step, transform.up) * turret.rotation;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            StandardBullet bullet = bulletObj.GetComponent<StandardBullet>();
            if (bullet != null)
            {
                bullet.Init(target, damage);
            }
        }
    }

    public void TakeDamage(int damageValue)
    {
        if (isTowerDead) return;

        health -= damageValue;
        if (health <= 0)
        {
            currentState = TowerState.Die;
        }
    }

    void Die()
    {
        if (isTowerDead) return;
        isTowerDead = true;

        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
