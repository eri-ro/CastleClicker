using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Projectile projectilePrefab;
    public float fireCooldown = 0.15f;
    float nextFireTime;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryFire();
        }
    }

    void TryFire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireCooldown;

        Vector3 mouseTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseTarget.z = 0f;

        Vector3 origin = transform.position;
        Vector2 dir = (mouseTarget - origin);

        if (dir.sqrMagnitude < 0.0001f) return;

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);

        int dmg = 1;
        if (GameManager.Instance != null)
            dmg = GameManager.Instance.damagePerShot;

        projectile.Initialize(dir, dmg);
    }
}