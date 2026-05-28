using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;
    public Transform[] Waypoints;

    void Awake() 
    {
        Instance = this;
    }
}
