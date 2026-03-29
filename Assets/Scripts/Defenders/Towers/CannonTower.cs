using UnityEngine;

// Single-enemy hit; splash is handled separately on the projectile.
public sealed class CannonTower : TowerBase
{
    readonly int damage;

    public CannonTower(int damageAmount)
    {
        damage = Mathf.Max(1, damageAmount);
    }

    public override void Attack(Enemy target)
    {
        if (target == null) return;
        TowerDamageChain.ApplyChainedDamage(ref target.hp, damage, target, out _, out _);
    }
}
