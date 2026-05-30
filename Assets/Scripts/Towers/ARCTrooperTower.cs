using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ARCTrooperTower : MonoBehaviour
{
    public GameObject soldierPrefab;
    public int soldierCount = 2;
    public float respawnTime = 8f;

    private List<GameObject> activeSoldiers = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < soldierCount; i++)
        {
            SpawnSoldier();
        }
    }

    void SpawnSoldier()
    {
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 1.5f;
        spawnPos.y = transform.position.y;

        GameObject soldier = Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
        ARCSoldier script = soldier.GetComponent<ARCSoldier>();
        if (script != null)
        {
            script.Initialize(transform.position);
        }
        activeSoldiers.Add(soldier);
    }

    // This would be called if we implement soldier health
    public void SoldierDied(GameObject soldier)
    {
        activeSoldiers.Remove(soldier);
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnSoldier();
    }
}
