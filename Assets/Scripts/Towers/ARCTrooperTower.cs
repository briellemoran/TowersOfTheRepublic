using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ARCTrooperTower : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject officerPrefab;
    public GameObject trooperPrefab;
    public int officerCount = 1;
    public int trooperCount = 2;
    public float respawnTime = 10f;

    private List<GameObject> activeSoldiers = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < officerCount; i++)
        {
            SpawnSoldier(officerPrefab);
        }
        for (int i = 0; i < trooperCount; i++)
        {
            SpawnSoldier(trooperPrefab);
        }
    }

    void OnDestroy()
    {
        foreach (var soldier in activeSoldiers)
        {
            if (soldier != null)
            {
                Destroy(soldier);
            }
        }
        activeSoldiers.Clear();
    }

    void SpawnSoldier(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 1.5f;
        spawnPos.y = transform.position.y;

        GameObject soldier = Instantiate(prefab, spawnPos, Quaternion.identity);
        ARCSoldier script = soldier.GetComponent<ARCSoldier>();
        if (script != null)
        {
            script.Initialize(this, transform.position);
            // We can store the prefab on the script if we need to respawn the exact type
            // For now, let's assume we can just check what type it was
        }
activeSoldiers.Add(soldier);
    }

    public void SoldierDied(GameObject soldier)
    {
        GameObject prefabToRespawn = trooperPrefab;
        // Check if it was an officer
        if (soldier.name.Contains("Officer"))
        {
            prefabToRespawn = officerPrefab;
        }

        activeSoldiers.Remove(soldier);
        StartCoroutine(RespawnRoutine(prefabToRespawn));
    }

    IEnumerator RespawnRoutine(GameObject prefab)
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnSoldier(prefab);
    }
}
