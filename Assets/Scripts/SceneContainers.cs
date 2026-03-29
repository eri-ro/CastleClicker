using UnityEngine;
// Scene parents so spawned enemies, shots, and turrets stay grouped in the hierarchy.
public class SceneContainers : MonoBehaviour
{
    public static SceneContainers Instance { get; private set; }

    public Transform enemies;
    public Transform projectiles;
    public Transform turrets;

    void Awake()
    {
        Instance = this;
    }
}
