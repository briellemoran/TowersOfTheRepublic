using UnityEngine;

public class TowerShoot : MonoBehaviour
{
    [Header("Stats")]
    public float damage = 25f;
    public float range = 8f;
    public float fireRate = 1.2f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform towerHead;
    public LayerMask enemyLayer;

    private EnemyHealth target;
    private float fireTimer;

    void Start()
    {
        // Look for targets twice a second
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }

    void Update()
    {
        // If we have a target and it's alive
        if (target != null && target.gameObject.activeInHierarchy == true)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            
            // If target is in range
            if (dist <= range)
            {
                RotateTowardTarget();

                // Handle Shooting timer
                fireTimer = fireTimer + Time.deltaTime;
                if (fireTimer >= 1f / fireRate)
                {
                    Shoot();
                    fireTimer = 0f;
                }
            }
            else
            {
                // Target walked away
                target = null;
            }
        }
    }

    void FindTarget()
    {
        // Find all enemies in range
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
        
        target = null;
        float shortestDistance = 1000f; // Very large number

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                target = enemy;
            }
        }
    }

    void RotateTowardTarget()
    {
        if (towerHead == null) return;

        // Look at the target's position
        Vector3 direction = target.transform.position - towerHead.position;
        direction.y = 0; // Keep the head level on the ground
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Smoothly rotate toward the target
            towerHead.rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Create the bullet
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Try to initialize it based on what script it has
        StandardBullet std = newBullet.GetComponent<StandardBullet>();
        if (std != null) std.Init(target, damage);

        AoEBullet aoe = newBullet.GetComponent<AoEBullet>();
        if (aoe != null) aoe.Init(target, damage);

        JediBullet jedi = newBullet.GetComponent<JediBullet>();
        if (jedi != null) jedi.Init(target, damage);
    }
}
