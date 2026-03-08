using UnityEngine;

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
