using UnityEngine;

public class StandardBullet : MonoBehaviour
{
    private EnemyHealth target;
    private float damage;
    public float speed = 12f;

    public void Init(EnemyHealth t, float dmg)
    {
        target = t;
        damage = dmg;
    }

    void Update()
    {
        // If target is gone, destroy the bullet
        if (target == null || target.gameObject.activeInHierarchy == false)
        {
            Destroy(gameObject);
            return;
        }

        // Move the bullet towards the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        // Check if we reached the target
        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
