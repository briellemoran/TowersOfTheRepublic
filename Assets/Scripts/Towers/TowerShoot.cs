using UnityEngine;
using System.Collections;

public class TowerShoot : MonoBehaviour
{
    public float damage = 25f;
    public float range = 4f;
    public float fireRate = 1.2f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public LayerMask enemyLayer;

    private EnemyHealth target;
    private bool isFiring = false;

    void Start()
    {
        InvokeRepeating(nameof(FindTarget), 0f, 0.5f);
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        target = null;
        float bestDist = float.MaxValue;

        foreach (Collider c in hits)
        {
            EnemyHealth e = c.GetComponent<EnemyHealth>();
            if (e == null) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < bestDist) { bestDist = d; target = e; }
        }

        if (target != null && !isFiring)
            StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        isFiring = true;
        while (target != null && target.gameObject.activeInHierarchy)
        {
            Shoot();
            yield return new WaitForSeconds(1f / fireRate);
        }
        isFiring = false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        b.GetComponent<Bullet>().Init(target, damage);
    }
}