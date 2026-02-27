using UnityEngine;

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

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);

        int dmg = 1;
        if (GameManager.Instance != null)
            dmg = GameManager.Instance.damagePerShot;

        projectile.Initialize(dir, dmg);
    }

    Enemy FindNearestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return null;

        Enemy best = null;
        float bestSqr = range * range;

        Vector3 pos = transform.position;

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject go = enemies[i];
            if (go == null) continue;

            float sqr = (go.transform.position - pos).sqrMagnitude;
            if (sqr <= bestSqr)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy != null)
                {
                    bestSqr = sqr;
                    best = enemy;
                }
            }
        }

        return best;
    }
}