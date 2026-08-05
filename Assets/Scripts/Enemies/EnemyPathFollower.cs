using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    public AudioClip baseSFX;
    public float speed = 3.5f;
    public int livesLost = 1;

    private NavMeshAgent agent;
    private Transform targetBase;

    // prevents ReachedBase() from firing more than once per enemy
    private bool hasReachedBase = false;

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
        if (hasReachedBase || agent == null || targetBase == null) return;

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
        if (hasReachedBase) return;
        hasReachedBase = true;

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
        hasReachedBase = false;

        // re-find the target if it was missed during instantiation
        if (targetBase == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("Target");
            if (targetObj != null)
            {
                targetBase = targetObj.transform;
            }
        }

        if (agent != null && agent.isOnNavMesh && targetBase != null)
        {
            agent.Warp(transform.position);
            agent.SetDestination(targetBase.position);
        }
    }
}