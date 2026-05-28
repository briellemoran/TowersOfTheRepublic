using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 60f;
    public int goldReward = 10;
    public bool isImmune = false;

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
        GameManager.Instance.AddGold(goldReward);
        EnemyManager.Instance.RemoveEnemy(this);
        // TODO: play death particle here
        gameObject.SetActive(false); // return to pool
    }
}