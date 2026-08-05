using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnLivesChanged;
    public static event Action OnGameOver;
    public static event Action OnWin;

    public int gold = 150;
    public int lives = 20;

    [Header("Debug")]
    [Tooltip("Logs every LoseLives call with a stack trace, so you can find what's calling it (useful for tracking down unexpected point loss).")]
    public bool debugLogLifeLoss = false;

    private bool isRoundOver = false;

    private const string TotalTimeKey = "TotalTimePlayed";
    private float unsavedElapsed = 0f;
    private float flushTimer = 0f;
    private const float FlushIntervalSeconds = 2f;

    void Awake()
    {
        Instance = this;
        isRoundOver = false;
        unsavedElapsed = 0f;
        flushTimer = 0f;
    }

    void Update()
    {
        unsavedElapsed += Time.deltaTime;
        flushTimer += Time.deltaTime;

        if (flushTimer >= FlushIntervalSeconds)
        {
            FlushElapsedTime();
        }
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
        if (isRoundOver) return;

        int previousLives = lives;
        lives -= amount;
        lives = Mathf.Max(lives, 0);

        if (debugLogLifeLoss)
        {
            Debug.Log($"[GameManager] LoseLives({amount}) called. Lives: {previousLives} -> {lives}\n" +
                      new System.Diagnostics.StackTrace());
        }

        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            isRoundOver = true;
            FlushElapsedTime();
            OnGameOver?.Invoke();
        }
    }

    public void TriggerWin()
    {
        if (isRoundOver) return;

        isRoundOver = true;
        FlushElapsedTime();
        OnWin?.Invoke();
    }

    public void FlushPlaytimeNow()
    {
        FlushElapsedTime();
    }

    private void FlushElapsedTime()
    {
        flushTimer = 0f;

        if (unsavedElapsed <= 0f) return;

        float total = PlayerPrefs.GetFloat(TotalTimeKey, 0f) + unsavedElapsed;
        PlayerPrefs.SetFloat(TotalTimeKey, total);
        PlayerPrefs.Save();
        unsavedElapsed = 0f;
    }

    void OnDestroy()
    {
        FlushElapsedTime();
    }

    void OnApplicationQuit()
    {
        FlushElapsedTime();
    }
}