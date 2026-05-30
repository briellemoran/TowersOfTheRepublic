using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tower Defense/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class WaveEntry
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
    }

    public List<WaveEntry> entries;
    public float delayBeforeWave;
}
