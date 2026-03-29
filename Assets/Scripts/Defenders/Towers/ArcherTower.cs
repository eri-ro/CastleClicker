using UnityEngine;

// Single-enemy hit (arrow-style).
public sealed class ArcherTower : TowerBase
{
    readonly int damage;

    public ArcherTower(int damageAmount)
    {
        damage = Mathf.Max(1, damageAmount);
    }

    public override void Attack(Enemy target)
    {
        if (target == null) return;
        TowerDamageChain.ApplyChainedDamage(ref target.hp, damage, target, out _, out _);
    }
}
