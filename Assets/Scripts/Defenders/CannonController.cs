using UnityEngine;
using UnityEngine.InputSystem;
// Player-aimed castle cannon: aim, fire rate, pierce, and splash from upgrades.
public class CannonController : MonoBehaviour
{
    public Projectile projectilePrefab;
    public InputActionReference mouseFire;
    public InputActionReference mouseLook;

    [Header("Aiming")]
    [Tooltip("Child transform at the cannon tip. Projectiles spawn here.")]
    public Transform projectileSpawnPoint;
    [Tooltip("Degrees to add to rotation. Use if sprite doesn't point right (0°) by default.")]
    public float rotationOffset;

    float nextFireTime;

    void Update()
    {
        if (!GameManager.Instance.gameStarted)
            return;

        UpdateRotation();

        // Use IsPressed (not triggered) so holding the button respects fire cooldown.
        // With "triggered", each shot needs a new click — human click rate hides the upgrade.
        if (mouseFire.action.IsPressed())
            TryFire();
    }

    void UpdateRotation()
    {
        Vector2 mouseScreen = mouseLook.action.ReadValue<Vector2>();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 pos = transform.position;
        float angle = Mathf.Atan2(mouseWorld.y - pos.y, mouseWorld.x - pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    void TryFire()
    {
        float cooldown = UpgradeManager.Instance.GetCannonFireCooldown();
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + cooldown;

        Vector2 mouseTarget = Camera.main.ScreenToWorldPoint(mouseLook.action.ReadValue<Vector2>());
        Vector2 origin = projectileSpawnPoint.position;
        Vector2 dir = (mouseTarget - origin);

        if (dir.sqrMagnitude < 0.0001f) return;

        Transform parent = SceneContainers.Instance.projectiles;
        int dmg = Mathf.Max(1, Mathf.RoundToInt((float)DefenderManager.Instance.GetDamage(DefenderType.CastleCannon)));
        bool piercing = UpgradeManager.Instance.HasCannonPiercing();
        float splashRadius = UpgradeManager.Instance.GetCannonSplashRadius();

        bool multiShot = UpgradeManager.Instance.HasCannonMultiShot();
        if (multiShot)
        {
            float spreadAngle = 8f * Mathf.Deg2Rad;
            FireProjectile(parent, origin, dir, dmg, piercing, splashRadius);
            FireProjectile(parent, origin, RotateVector(dir, spreadAngle), dmg, piercing, splashRadius);
            FireProjectile(parent, origin, RotateVector(dir, -spreadAngle), dmg, piercing, splashRadius);
        }
        else
        {
            FireProjectile(parent, origin, dir, dmg, piercing, splashRadius);
        }
    }

    void FireProjectile(Transform parent, Vector2 origin, Vector2 dir, int dmg, bool piercing, float splashRadius)
    {
        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity, parent);
        projectile.Initialize(dir, dmg, piercing, splashRadius, ProjectileDamageProfile.Cannon);
    }

    static Vector2 RotateVector(Vector2 v, float rad)
    {
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}
