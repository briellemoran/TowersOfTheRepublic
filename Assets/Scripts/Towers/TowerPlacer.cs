using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance;

    [Header("Settings")]
    public LayerMask terrainLayer;
    public LayerMask pathLayer;
    public LayerMask buildZoneLayer;
    public Material ghostValidMaterial;
    public Material ghostInvalidMaterial;

    [Header("Current Selection")]
    public TowerData selectedData;
    private GameObject ghostInstance;
    private BuildZone currentBuildZone;

    void Awake() 
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; 
        selectedData = null; 
        Debug.Log("[Placer] Instance initialized.");
    }

    void Start()
    {
        selectedData = null;
    }

    void Update()
    {
        if (selectedData == null)
        {
            if (ghostInstance != null) Destroy(ghostInstance);
            return;
        }

        // Just log selection status once in a while
        if (Time.frameCount % 120 == 0) Debug.Log("[Placer] Waiting for placement of " + selectedData.towerName);

        UpdateGhost();

        if (Input.GetMouseButtonDown(0))
        {
            // Check if clicking on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TryPlaceTower();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    void UpdateGhost()
    {
        if (ghostInstance == null)
        {
            if (selectedData.ghostPrefab == null)
            {
                Debug.LogError("[Placer] ghostPrefab is missing on " + selectedData.towerName);
                return;
            }

            ghostInstance = Instantiate(selectedData.ghostPrefab);
            
            // Disable ALL colliders on ghost immediately
            Collider[] cols = ghostInstance.GetComponentsInChildren<Collider>();
            foreach (var c in cols) c.enabled = false;

            // Move to Ignore Raycast layer
            int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
            if (ignoreLayer != -1)
            {
                ghostInstance.layer = ignoreLayer;
                foreach (Transform t in ghostInstance.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = ignoreLayer;
                }
            }
            
            Debug.Log("[Placer] Ghost instantiated for " + selectedData.towerName);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = terrainLayer | buildZoneLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, mask))
        {
            ghostInstance.transform.position = hit.point;

            bool isValid = IsPlacementValid(hit.point);
            SetGhostMaterial(isValid);
        }
        else
        {
            ghostInstance.transform.position = new Vector3(0, -100, 0);
        }
    }

    bool IsPlacementValid(Vector3 position)
    {
        // 1. Check if near an unoccupied BuildZone
        Collider[] zones = Physics.OverlapSphere(position, 4.0f, buildZoneLayer);
        currentBuildZone = null;

        foreach (var col in zones)
        {
            BuildZone bz = col.GetComponent<BuildZone>() ?? col.GetComponentInParent<BuildZone>();
            if (bz != null && !bz.isOccupied)
            {
                currentBuildZone = bz;
                break;
            }
        }

        if (currentBuildZone == null) return false;

        // 2. Check if on path
        if (Physics.CheckSphere(position, 0.05f, pathLayer)) return false;

        // 3. Check for overlapping towers
        Collider[] overlaps = Physics.OverlapSphere(position, 0.5f);
        foreach (var col in overlaps)
        {
            if (col.gameObject == ghostInstance) continue;
            if (col.GetComponent<TowerShoot>() != null || col.GetComponent<ARCTrooperTower>() != null || col.GetComponentInParent<TowerShoot>() != null)
            {
                return false;
            }
        }

        return true;
    }

    void TryPlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, terrainLayer | buildZoneLayer))
        {
            TryPlaceAt(hit.point);
        }
    }

    public void TryPlaceAt(Vector3 position)
    {
        if (selectedData == null) return;

        if (IsPlacementValid(position))
        {
            if (GameManager.Instance.SpendGold(selectedData.cost))
            {
                Vector3 spawnPos = currentBuildZone.transform.position;
                Instantiate(selectedData.towerPrefab, spawnPos, Quaternion.identity);
                currentBuildZone.isOccupied = true;
                
                Debug.Log("[Placer]: Placed " + selectedData.towerName + " on " + currentBuildZone.name);
                CancelPlacement();
            }
            else
            {
                Debug.Log("[Placer] failed: Not enough gold.");
            }
        }
    }

    void SetGhostMaterial(bool valid)
    {
        if (ghostInstance == null) return;
        MeshRenderer[] renderers = ghostInstance.GetComponentsInChildren<MeshRenderer>();
        Material mat = valid ? ghostValidMaterial : ghostInvalidMaterial;
        foreach (var r in renderers)
        {
            r.material = mat;
        }
    }

    public void CancelPlacement()
    {
        selectedData = null;
        if (ghostInstance != null) Destroy(ghostInstance);
    }

    public void SelectTower(TowerData data)
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        selectedData = data;
        if (data != null) Debug.Log("[Placer] Tower Selected: " + data.towerName);
    }
}
