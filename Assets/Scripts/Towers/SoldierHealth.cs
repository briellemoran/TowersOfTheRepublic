using UnityEngine;
using UnityEngine.UI;

public class SoldierHealth : MonoBehaviour
{
    public float maxHP = 50f;
    private float currentHP;

    [Header("UI")]
    public Slider healthBarSlider;

    private ARCSoldier soldier;

    void Awake()
    {
        soldier = GetComponent<ARCSoldier>();
        currentHP = maxHP;
        if (healthBarSlider != null)
        {
            healthBarSlider.value = 1f;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0f);

        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHP / maxHP;
        }

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (soldier != null)
        {
            soldier.OnDeath();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
