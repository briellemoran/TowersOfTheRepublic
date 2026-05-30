using UnityEngine;

public class SpeedModifier : MonoBehaviour
{
    private EnemyPathFollower pathFollower;
    private float originalSpeed;
    private float duration;
    private float timer;

    public void Apply(float slowAmount, float slowDuration)
    {
        if (pathFollower == null)
        {
            pathFollower = GetComponent<EnemyPathFollower>();
            originalSpeed = pathFollower.speed;
        }

        duration = slowDuration;
        timer = duration;

        // Apply slow
        pathFollower.speed = originalSpeed * slowAmount;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Remove();
        }
    }

    public void RefreshTimer()
    {
        timer = duration;
    }

    private void Remove()
    {
        if (pathFollower != null)
        {
            pathFollower.speed = originalSpeed;
        }
        Destroy(this);
    }
}
