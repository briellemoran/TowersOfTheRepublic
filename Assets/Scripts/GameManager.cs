using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // Events — UIManager subscribes to these
    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnLivesChanged;
    public static event Action OnGameOver;
    public static event Action OnWin;
    public int gold = 150;
    public int lives = 20;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }
    
    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }
    
    public void LoseLives(int amount)
    {
        lives -= amount;
        lives = Mathf.Max(lives, 0);
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0) {
            OnGameOver?.Invoke();
        }
    }

    public void TriggerWin()
    {
        OnWin?.Invoke();
    }
}
