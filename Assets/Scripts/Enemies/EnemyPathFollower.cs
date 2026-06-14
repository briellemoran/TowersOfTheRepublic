using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    public AudioClip baseSFX;
    public float speed = 3.5f;
    public int livesLost = 1;
    
    private NavMeshAgent agent;
    private Transform targetBase;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    void Start()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag("Target");
        if (targetObj != null)
        {
            targetBase = targetObj.transform;
            agent.SetDestination(targetBase.position);
        }
        else
        {
            Debug.LogWarning("Target base not found by tag 'Target'!");
        }
    }

    void Update()
    {
        if (agent == null || targetBase == null) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                ReachedBase();
            }
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

    void OnDisable()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    void OnEnable()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public void ResetPath()
    {
        // Re-find target if it was missed during instantiation (e.g. tag was missing)
        if (targetBase == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("Target");
            if (targetObj != null)
            {
                targetBase = targetObj.transform;
            }
        }

        if (agent != null && targetBase != null)
        {
            agent.Warp(transform.position); // Ensure agent is on NavMesh
            agent.SetDestination(targetBase.position);
        }
    }
}