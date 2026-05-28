using UnityEngine;
public class EnemyPathFollower : MonoBehaviour
{
    public float speed = 3.5f;
    public int livesLost = 1;
    public int waypointIndex = 0;
    
    private Transform[] waypoints;
    
    void Start()
    {
        waypoints = PathManager.Instance.Waypoints;
        transform.position = waypoints[0].position;
    }
    
    void Update()
    {
        if (waypointIndex >= waypoints.Length)
        {
            ReachedBase();
            return;
        }

        Transform target = waypoints[waypointIndex];
        // move toward the current waypoint
        transform.position = Vector3.MoveTowards(
        transform.position,
        target.position,
        speed * Time.deltaTime
        );

        // rotate to face direction of travel
        Vector3 dir = target.position - transform.position;
        if (dir != Vector3.zero)
        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(dir),
        10f * Time.deltaTime
        );

        // go to the next waypoint when close enough
        if (Vector3.Distance(transform.position, target.position) < 0.15f){
            waypointIndex++;
        }
    }

    void ReachedBase()
    {
        GameManager.Instance.LoseLives(livesLost);
        EnemyManager.Instance.RemoveEnemy(GetComponent<EnemyHealth>());
        gameObject.SetActive(false);
    }
 
    public void ResetPath()
    {
        waypointIndex = 0;
        transform.position = waypoints[0].position;
    }
}