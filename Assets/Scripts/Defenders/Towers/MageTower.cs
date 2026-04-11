using System.Collections.Generic;
using UnityEngine;

// Hit one enemy, then splash damage nearby
public sealed class MageTower : TowerBase
{
    readonly int primaryDamage;
    readonly float splashRadius;
    readonly float splashDamageMultiplier;

    public MageTower(int primaryDamage, float splashRadius = 2.5f, float splashDamageMultiplier = 0.45f)
    {
        this.primaryDamage = Mathf.Max(1, primaryDamage);
        this.splashRadius = splashRadius;
        this.splashDamageMultiplier = Mathf.Clamp01(splashDamageMultiplier);
    }

    public override void Attack(Enemy target)
    {
        if (target == null) return;

        Vector2 center = target.transform.position;
        TowerDamageChain.ApplyChainedDamage(ref target.hp, primaryDamage, target, out _, out _);

        int splashDmg = Mathf.Max(1, Mathf.RoundToInt(primaryDamage * splashDamageMultiplier));
        List<Enemy> snapshot = new List<Enemy>(EnemyRegistry.Instance.activeEnemies);
        foreach (Enemy e in snapshot)
        {
            if (e == null || e == target) continue;
            if (Vector2.SqrMagnitude((Vector2)e.transform.position - center) > splashRadius * splashRadius)
                continue;
            TowerDamageChain.ApplyChainedDamage(ref e.hp, splashDmg, e, out _, out _);
        }
    }
}
