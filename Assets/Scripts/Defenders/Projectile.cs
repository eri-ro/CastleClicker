using System.Collections.Generic;
using UnityEngine;
// Flying shot; on hit, uses archer/cannon/mage style damage and optional splash.
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 2.0f;
    int damage;
    bool piercing;
    float splashRadius;
    ProjectileDamageProfile damageProfile;
    int piercesRemaining;
    HashSet<Enemy> hitEnemies = new HashSet<Enemy>();
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction, int damageAmount, bool isPiercing = false, float splash = 0f,
        ProjectileDamageProfile profile = ProjectileDamageProfile.Cannon)
    {
        damage = damageAmount;
        piercing = isPiercing;
        splashRadius = splash;
        damageProfile = profile;
        piercesRemaining = isPiercing ? 2 : 1;

        Vector2 dir = direction.normalized;
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null || hitEnemies.Contains(enemy)) return;

        hitEnemies.Add(enemy);

        if (damageProfile == ProjectileDamageProfile.Mage)
        {
            new MageTower(damage, splashRadius > 0f ? splashRadius : 2.5f).Attack(enemy);
            piercesRemaining--;
            if (piercesRemaining <= 0)
                Destroy(gameObject);
            return;
        }

        if (damageProfile == ProjectileDamageProfile.Cannon)
            new CannonTower(damage).Attack(enemy);
        else
            new ArcherTower(damage).Attack(enemy);

        if (splashRadius > 0f && damageProfile == ProjectileDamageProfile.Cannon)
        {
            Vector2 splashCenter = enemy.transform.position;
            CannonSplashVfx.Play(splashCenter, splashRadius);
            DealSplashDamage(splashCenter);
        }

        piercesRemaining--;
        if (piercesRemaining <= 0)
            Destroy(gameObject);
    }

    void DealSplashDamage(Vector2 center)
    {
        // Copy list first: splash can kill enemies, which Unregister from activeEnemies during damage.
        List<Enemy> snapshot = new List<Enemy>(EnemyRegistry.Instance.activeEnemies);
        foreach (Enemy e in snapshot)
        {
            if (e == null || hitEnemies.Contains(e)) continue;
            float dist = Vector2.Distance(e.transform.position, center);
            if (dist <= splashRadius)
            {
                int splashDmg = Mathf.Max(1, Mathf.RoundToInt(damage * (1f - dist / splashRadius * 0.5f)));
                TowerDamageChain.ApplyChainedDamage(ref e.hp, splashDmg, e, out _, out _);
            }
        }
    }
}
