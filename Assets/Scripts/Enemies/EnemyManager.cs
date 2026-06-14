using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private List<EnemyHealth> activeEnemies = new List<EnemyHealth>();

    void Awake() { Instance = this; }

    public void AddEnemy(EnemyHealth e) { activeEnemies.Add(e); }

    public void RemoveEnemy(EnemyHealth e) { activeEnemies.Remove(e); }

    public int ActiveCount() { return activeEnemies.Count; }
    
    // returns the enemy closest to a world position (used by towers)
    public EnemyHealth GetNearest(Vector3 position)
    {
        EnemyHealth nearest = null;
        float bestDist = float.MaxValue;
        
        foreach (EnemyHealth e in activeEnemies)
        {
            if (e == null) {
                continue;
            }
        
            float dist = Vector3.Distance(position, e.transform.position);
        
            if (dist < bestDist)
            {
                bestDist = dist;
                nearest = e;
            }
        }
        return nearest;
    }

    // returns the enemy furthest along the path (highest waypointIndex)
    public EnemyHealth GetFirst()
    {
        EnemyHealth first = null;
        float bestDist = float.MaxValue;

        foreach (EnemyHealth e in activeEnemies)
        {
            if (e == null) {
                continue;
            }

            UnityEngine.AI.NavMeshAgent agent = e.GetComponent<UnityEngine.AI.NavMeshAgent>();
            float dist = agent != null && !agent.pathPending ? agent.remainingDistance : float.MaxValue;
            
            if (dist < bestDist) 
            { 
                bestDist = dist; first = e; 
            }
        }
        return first;
    }
}