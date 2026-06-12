using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    public GameObject enemyPrefab;
    public int poolSize = 30;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        // create all enemies disabled
        for (int i = 0; i < poolSize; i++)
        {
        GameObject obj = Instantiate(enemyPrefab);
        obj.SetActive(false);
        pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 spawnPosition)
    {
        GameObject obj;
        if (pool.Count > 0) {
            obj = pool.Dequeue();
        }
        else {
            obj = Instantiate(enemyPrefab); // grow if needed
        }

        obj.transform.position = spawnPosition;
        obj.GetComponent<EnemyHealth>().isImmune = false;
        obj.GetComponent<EnemyPathFollower>().ResetPath();
        obj.SetActive(true);
        EnemyManager.Instance.AddEnemy(obj.GetComponent<EnemyHealth>());
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}