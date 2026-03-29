// Applies damage and reports kill + coin reward for towers and splash.
public static class TowerDamageChain
{
    public static void ApplyChainedDamage(ref int health, int damage, Enemy enemy, out bool killed, out double goldEarnedIfKill)
    {
        killed = false;
        goldEarnedIfKill = 0;
        if (enemy == null || damage <= 0) return;

        // Delegate to the enemy; if still alive, copy HP back for splash-style callers.
        enemy.TakeDamage(damage, out killed, out goldEarnedIfKill);
        if (!killed && enemy != null)
            health = enemy.hp;
    }
}
