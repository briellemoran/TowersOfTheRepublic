using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    public Material highlightMaterial;
    public GameObject towerPrefab;

    private Renderer tileRenderer;
    private Material originalMaterial;
    public bool tileOccupied = false; // Changed to public so TowerPlacer can check it
    private GameObject placedTower = null;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalMaterial = tileRenderer.material;
        }
    }

    void OnMouseOver()
    {
        if (!tileOccupied && highlightMaterial != null)
        {
            tileRenderer.material = highlightMaterial;
        }
    }

    void OnMouseExit()
    {
        if (!tileOccupied && originalMaterial != null)
        {
            tileRenderer.material = originalMaterial;
        }
    }

    void OnMouseDown()
    {
        if (TowerPlacer.Instance != null && TowerPlacer.Instance.selectedData != null)
        {
            return;
        }

        // The old fallback logic was causing 'random' towers to be placed when selection was null.
        // removed to ensure all building goes through the selection system.
    }

    public void PlaceTower(GameObject prefab)
    {
        if (tileOccupied) return;

        // Instantiate at parent position
        placedTower = Instantiate(prefab, transform.parent.position, transform.parent.rotation);
        tileOccupied = true;
        
        // Face the road
        FaceNearestPath(placedTower);

        // Update color to show it's occupied
        if (highlightMaterial != null)
        {
            tileRenderer.material = highlightMaterial;
        }
        
        // Also inform any other logic that this zone is occupied
        BuildZone bz = GetComponent<BuildZone>() ?? GetComponentInParent<BuildZone>();
        if (bz != null) bz.isOccupied = true;
    }

    void FaceNearestPath(GameObject tower)
    {
        // Look for the closest object on the Path layer
        int pathLayer = LayerMask.GetMask("Path");
        Collider[] pathHits = Physics.OverlapSphere(tower.transform.position, 15f, pathLayer);
        
        Vector3 closestPoint = Vector3.zero;
        bool found = false;
        float minDist = Mathf.Infinity;

        foreach (var hit in pathHits)
        {
            // Try to find the closest point on the collider
            Vector3 point = hit.ClosestPoint(tower.transform.position);
            float dist = Vector3.Distance(tower.transform.position, point);
            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = point;
                found = true;
            }
        }

        if (found)
        {
            Vector3 direction = closestPoint - tower.transform.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                tower.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
