using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public WaveData[] waves;
    public Transform spawnPoint;
    
    public int currentWave = 0;
    public bool waveActive = false;

    // Events for UI (beginner friendly use of Actions)
    public static event Action<int> OnWaveStarted;
    public static event Action OnWaveComplete;
    public static event Action OnAllWavesComplete;

    private int enemiesRemainingInWave = 0;
    private bool isSpawning = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartNextWave()
    {
        if (waveActive == true) return;

        if (currentWave >= waves.Length)
        {
            GameManager.Instance.TriggerWin();
            if (OnAllWavesComplete != null) OnAllWavesComplete();
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWave]));
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        waveActive = true;
        isSpawning = true;
        currentWave = currentWave + 1;
        
        // Fire UI event
        if (OnWaveStarted != null) OnWaveStarted(currentWave);

        yield return new WaitForSeconds(wave.delayBeforeWave);

        enemiesRemainingInWave = 0;
        for (int i = 0; i < wave.entries.Count; i++)
        {
            enemiesRemainingInWave = enemiesRemainingInWave + wave.entries[i].count;
        }

        for (int j = 0; j < wave.entries.Count; j++)
        {
            WaveData.WaveEntry entry = wave.entries[j];
            for (int k = 0; k < entry.count; k++)
            {
                Instantiate(entry.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }

        isSpawning = false;
    }

    public void OnEnemyRemoved()
    {
        enemiesRemainingInWave = enemiesRemainingInWave - 1;

        if (enemiesRemainingInWave <= 0 && isSpawning == false)
        {
            waveActive = false;
            
            // Fire UI event
            if (OnWaveComplete != null) OnWaveComplete();

            if (currentWave >= waves.Length)
            {
                if (OnAllWavesComplete != null) OnAllWavesComplete();
                GameManager.Instance.TriggerWin();
            }
        }
    }
}
