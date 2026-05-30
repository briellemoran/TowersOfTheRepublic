using UnityEngine;

public class JediBullet : MonoBehaviour
{
    private EnemyHealth target;
    private float damage;

    public float speed = 12f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;

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
            target.TakeDamage(damage);

            // Apply the slow effect
            SpeedModifier modifier = target.GetComponent<SpeedModifier>();
            if (modifier == null)
            {
                // Add the component if they don't have it
                modifier = target.gameObject.AddComponent<SpeedModifier>();
                modifier.Apply(slowAmount, slowDuration);
            }
            else
            {
                // Just refresh the timer if they are already slowed
                modifier.RefreshTimer();
            }

            Destroy(gameObject);
        }
    }
}
