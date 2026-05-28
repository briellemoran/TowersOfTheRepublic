using UnityEngine;

public class Bullet : MonoBehaviour
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
        if (target == null || !target.gameObject.activeInHierarchy)
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
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}