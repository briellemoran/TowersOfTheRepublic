using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int cost;
    public float damage;
    public float range;
    public float fireRate;    // shots per second
    public GameObject towerPrefab;
    public GameObject ghostPrefab;
    public Sprite shopIcon;
}