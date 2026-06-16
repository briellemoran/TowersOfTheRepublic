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

    void OnEnable() // called when pulled from pool
    {
        currentHP = maxHP;
        if (healthBarSlider != null) {
            healthBarSlider.value = 1f;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isImmune) {
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
        // for the droid animation death
        DroidAnimationController animController = GetComponentInChildren<DroidAnimationController>();
        if (animController != null)
        {
            animController.TriggerDeath();
        }

        if (deathParticle != null) Instantiate(deathParticle, transform.position, transform.rotation);
        
        if (deathSFX != null) AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        
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