using System.Collections.Generic;
using UnityEngine;
// Auto-turret: shoots the nearest enemy on a timer.
public class Turret : MonoBehaviour
{
    public Projectile projectilePrefab;
    public float range = 6f;
    public float fireInterval = 0.6f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        Enemy target = FindNearestEnemyInRange();
        if (target == null) return;

        timer = 0f;

        Vector3 origin = transform.position;
        Vector2 dir = (target.transform.position - origin);

        if (dir.sqrMagnitude < 0.0001f) return;

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity, SceneContainers.Instance.projectiles);

        int dmg = Mathf.Max(1, Mathf.RoundToInt((float)DefenderManager.Instance.GetDamage(DefenderType.Turret)));

        projectile.Initialize(dir, dmg, false, 0f, ProjectileDamageProfile.Turret);
    }

    Enemy FindNearestEnemyInRange()
    {
        List<Enemy> enemies = EnemyRegistry.Instance.activeEnemies;
        if (enemies.Count == 0) return null;

        Enemy best = null;
        float bestSqr = range * range;
        Vector3 pos = transform.position;

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];

            float sqr = (enemy.transform.position - pos).sqrMagnitude;
            if (sqr <= bestSqr)
            {
                bestSqr = sqr;
                best = enemy;
            }
        }

        return best;
    }
}
