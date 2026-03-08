using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    public Projectile projectilePrefab;
    public float fireCooldown = 0.15f;
    float nextFireTime;
    public InputActionReference mouseFire;
    public InputActionReference mouseLook;

    void Update()
    {
        if (mouseFire.action.triggered)
        {
            TryFire();
        }
    }

    void TryFire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireCooldown;

        Vector2 mouseTarget = Camera.main.ScreenToWorldPoint(mouseLook.action.ReadValue<Vector2>());

        Vector2 origin = transform.position;
        Vector2 dir = (mouseTarget - origin);

        if (dir.sqrMagnitude < 0.0001f) return;

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity, SceneContainers.Instance.projectiles);

        int dmg = 1;
        dmg = Mathf.Max(1, Mathf.RoundToInt((float)GameManager.Instance.castleDamage));

        projectile.Initialize(dir, dmg);
    }
}