using UnityEngine;
public class EnemyPathFollower : MonoBehaviour
{
    public AudioClip baseSFX;
    public float speed = 3.5f;
    public int livesLost = 1;
    public int waypointIndex = 0;
    
    private Transform[] waypoints;
    
    private void EnsureWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            if (PathManager.Instance != null)
            {
                waypoints = PathManager.Instance.Waypoints;
            }
        }
    }

    void Start()
    {
        EnsureWaypoints();
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }
    
    void Update()
    {
        EnsureWaypoints();
        if (waypoints == null || waypoints.Length == 0 || waypointIndex >= waypoints.Length)
        {
            if (waypoints != null && waypointIndex >= waypoints.Length)
            {
                ReachedBase();
            }
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
        if (baseSFX != null) AudioSource.PlayClipAtPoint(baseSFX, transform.position);
        GameManager.Instance.LoseLives(livesLost);
        EnemyManager.Instance.RemoveEnemy(GetComponent<EnemyHealth>());
        
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyRemoved();
        }
        
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.Return(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    public void ResetPath()
    {
        EnsureWaypoints();
        waypointIndex = 0;
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }
}