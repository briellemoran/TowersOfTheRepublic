using UnityEngine;
using UnityEngine.AI;

public class DroidAnimationController : MonoBehaviour
{
    public float moveThreshold = 0.1f;

    private Animator animator;
    private NavMeshAgent agent;
    private EnemyPathFollower pathFollower;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
        pathFollower = GetComponentInParent<EnemyPathFollower>();
    }

    void Update()
    {
        // safety check to ensure animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null) return;

        // get the current speed from the NavMeshAgent
        float currentSpeed = 0f;
        if (agent != null)
        {
            currentSpeed = agent.velocity.magnitude;
        }

        // check if the droid is attacking
        // (the EnemyAttack script disables the EnemyPathFollower when it starts attacking)
        bool isAttacking = false;
        if (pathFollower != null)
        {
            if (pathFollower.enabled == false)
            {
                isAttacking = true;
            }
        }

        // update the Animator parameters
        // setting speed directly makes it easier for the animator to blend
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("Attacking", isAttacking);
    }

    // the droid shrinks when it dies
    public void TriggerDeath()
    {
        if(animator != null){
            animator.SetTrigger("Die");
        }
    }
}
