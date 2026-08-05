using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 60f;
    public int goldReward = 10;
    public bool isImmune = false;
    public AudioClip deathSFX;
    public GameObject deathParticle;

    private AudioSource audioSource;

    [Header("UI")]
    public Slider healthBarSlider;
    private float currentHP;

    private bool isDead = false;

    void OnEnable()
    {
        currentHP = maxHP;
        isDead = false;

        if (healthBarSlider != null) {
            healthBarSlider.value = 1f;
        }

        EnemyPathFollower follower = GetComponent<EnemyPathFollower>();
        if (follower != null) follower.enabled = true;
    }

    public void TakeDamage(float amount)
    {
        if (isImmune || isDead) {
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0f);

        if (healthBarSlider != null) {
            healthBarSlider.value = currentHP / maxHP;
        }

        if (currentHP <= 0f) {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // stop the enemy from moving and disable path-following immediately
        EnemyPathFollower follower = GetComponent<EnemyPathFollower>();
        if (follower != null) {
            follower.enabled = false;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh) {
            agent.isStopped = true;
        }

        // for the droid animation death
        DroidAnimationController animController = GetComponentInChildren<DroidAnimationController>();
        if (animController != null)
        {
            animController.TriggerDeath();
        }

        if (deathParticle != null) {
            Instantiate(deathParticle, transform.position, transform.rotation);
        }

        if (deathSFX != null) {
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        }

        GameManager.Instance.AddGold(goldReward);
        EnemyManager.Instance.RemoveEnemy(this);
        WaveManager.Instance.OnEnemyRemoved();

        // delay the destruction so the animation can run
        Invoke("ReturnToPool", 1f);
    }

    void ReturnToPool()
    {
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.Return(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}