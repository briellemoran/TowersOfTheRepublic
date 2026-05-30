using UnityEngine;

public class AoEBullet : MonoBehaviour
{
    private EnemyHealth target;
    private float damage;
    
    public float speed = 10f;
    public float splashRadius = 2f;
    public LayerMask enemyLayer;

    public void Init(EnemyHealth t, float dmg)
    {
        target = t;
        damage = dmg;
    }

    void Update()
    {
        if (target == null || target.gameObject.activeInHierarchy == false)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Find all objects in the splash radius
        Collider[] enemies = Physics.OverlapSphere(transform.position, splashRadius, enemyLayer);
        
        // Loop through everything we hit
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyHealth enemy = enemies[i].GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
