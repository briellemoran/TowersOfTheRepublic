using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance;

    public TowerData selectedData;
    public LayerMask terrainLayer;
    public LayerMask pathLayer;

    void Awake() { Instance = this; }

    void Update()
    {
        if (selectedData == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, terrainLayer))
            {
                bool onPath = Physics.CheckSphere(hit.point, 0.8f, pathLayer);
                if (!onPath && GameManager.Instance.SpendGold(selectedData.cost))
                {
                    Instantiate(selectedData.towerPrefab, hit.point, Quaternion.identity);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
            selectedData = null;
    }

    public void SelectTower(TowerData data)
    {
        selectedData = data;
    }
}