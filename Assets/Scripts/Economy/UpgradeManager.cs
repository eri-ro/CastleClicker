using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Tooltip("Assign upgrade assets here. Order = runtime order. If empty, default upgrades are created in code.")]
    [SerializeField] List<UpgradeDefinition> upgradeDefinitions = new List<UpgradeDefinition>();

    [HideInInspector]
    public List<Upgrade> upgrades = new List<Upgrade>();

    const double LavaMoatDpsBase = 5;
    const double LavaMoatDpsGrowth = 1.25;
    const double KnightDamagePerKnight = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var definitions = (upgradeDefinitions != null && upgradeDefinitions.Count > 0)
            ? upgradeDefinitions
            : GetDefaultDefinitions();
        BuildUpgradesFromDefinitions(definitions);
    }

    void BuildUpgradesFromDefinitions(IEnumerable<UpgradeDefinition> definitions)
    {
        upgrades.Clear();
        foreach (UpgradeDefinition def in definitions)
            upgrades.Add(def.ToUpgrade());
    }

    /// <summary>
    /// Fallback when no UpgradeDefinition assets are assigned. Creates in-memory definitions
    /// so we still use the single path: definition -> ToUpgrade() -> runtime list.
    /// </summary>
    static List<UpgradeDefinition> GetDefaultDefinitions()
    {
        var list = new List<UpgradeDefinition>();

        list.Add(CreateDef("Coin Income I", ResourceType.Coins, 25, 1.25, 1, UpgradeState.Available, UpgradeEffectType.Income, ResourceType.Coins, 1.5));
        list.Add(CreateDef("Mana Income I", ResourceType.Coins, 40, 1.30, 1, UpgradeState.Available, UpgradeEffectType.Income, ResourceType.Mana, 2.0));
        list.Add(CreateDef("Coin Income II", ResourceType.Coins, 100, 1.35, 2, UpgradeState.Locked, UpgradeEffectType.Income, ResourceType.Coins, 2.0));
        list.Add(CreateDef("Mana Income II", ResourceType.Coins, 150, 1.40, 2, UpgradeState.Locked, UpgradeEffectType.Income, ResourceType.Mana, 2.5));
        list.Add(CreateDef("Castle Damage", ResourceType.Coins, 10, 1.15, 0, UpgradeState.Available, UpgradeEffectType.CastleDamage, ResourceType.Coins, 1));
        list.Add(CreateDef("Turret Damage", ResourceType.Coins, 25, 1.18, 0, UpgradeState.Available, UpgradeEffectType.TurretDamage, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Turret", ResourceType.Coins, 50, 1.25, 0, UpgradeState.Available, UpgradeEffectType.BuyTurret, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Moat", ResourceType.Coins, 150, 1, 0, UpgradeState.Available, UpgradeEffectType.BuyMoat, ResourceType.Coins, 1));
        list.Add(CreateDef("Lava Moat Unlock", ResourceType.Coins, 300, 1, 0, UpgradeState.Locked, UpgradeEffectType.LavaMoatUnlock, ResourceType.Coins, 1));
        list.Add(CreateDef("Lava Moat DPS", ResourceType.Coins, 100, 1.22, 0, UpgradeState.Locked, UpgradeEffectType.LavaMoatDps, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Knight", ResourceType.Coins, 75, 1.22, 0, UpgradeState.Available, UpgradeEffectType.BuyKnight, ResourceType.Coins, 1));

        return list;
    }

    static UpgradeDefinition CreateDef(string name, ResourceType costType, double baseCost, double growth, int tier, UpgradeState state, UpgradeEffectType effectType, ResourceType targetType, double multiplier)
    {
        var def = ScriptableObject.CreateInstance<UpgradeDefinition>();
        def.upgradeName = name;
        def.costResourceType = costType;
        def.baseCost = baseCost;
        def.growthRate = growth;
        def.tier = tier;
        def.initialState = state;
        def.effectType = effectType;
        def.targetResourceType = targetType;
        def.multiplier = multiplier;
        return def;
    }

    public bool TryPurchaseUpgrade(int index)
    {
        if (index < 0 || index >= upgrades.Count)
            return false;

        Upgrade upgrade = upgrades[index];
        bool repeatable = IsRepeatable(upgrade);
        bool oneShotPurchased = IsOneShot(upgrade) && upgrade.level > 0;

        if (!repeatable && upgrade.state != UpgradeState.Available)
            return false;
        if (oneShotPurchased)
            return false;
        if (upgrade.effect.effectType == UpgradeEffectType.BuyTurret && upgrade.level >= 16)
            return false;

        double cost = upgrade.GetCost();
        if (!SpendUpgradeCost(upgrade.costResourceType, cost))
            return false;

        upgrade.level++;
        ApplyUpgradeEffect(upgrade);

        if (upgrade.effect.effectType == UpgradeEffectType.Income)
        {
            upgrade.state = UpgradeState.Purchased;
            UnlockNextTier(upgrade);
        }
        else if (IsOneShot(upgrade))
            upgrade.state = UpgradeState.Purchased;

        GameManager.Instance.NotifyUpgradeSystemChanged();
        return true;
    }

    static bool IsRepeatable(Upgrade u)
    {
        return u.effect.effectType != UpgradeEffectType.Income
            && u.effect.effectType != UpgradeEffectType.BuyMoat
            && u.effect.effectType != UpgradeEffectType.LavaMoatUnlock;
    }

    static bool IsOneShot(Upgrade u)
    {
        return u.effect.effectType == UpgradeEffectType.BuyMoat
            || u.effect.effectType == UpgradeEffectType.LavaMoatUnlock;
    }

    bool SpendUpgradeCost(ResourceType resourceType, double amount)
    {
        switch (resourceType)
        {
            case ResourceType.Coins:
                return ResourceManager.Instance.SpendCoins(amount);

            case ResourceType.Mana:
                return ResourceManager.Instance.SpendResource(ResourceType.Mana, amount);

            default:
                return false;
        }
    }

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        switch (upgrade.effect.effectType)
        {
            case UpgradeEffectType.Income:
                if (upgrade.effect.targetResourceType == ResourceType.Coins)
                    ResourceManager.Instance.SetCoinsPerSecond(
                        ResourceManager.Instance.GetCoinsPerSecond() * upgrade.effect.multiplier);
                else if (upgrade.effect.targetResourceType == ResourceType.Mana)
                    ResourceManager.Instance.SetManaPerSecond(
                        ResourceManager.Instance.GetManaPerSecond() * upgrade.effect.multiplier);
                break;

            case UpgradeEffectType.CastleDamage:
            case UpgradeEffectType.TurretDamage:
            case UpgradeEffectType.LavaMoatDps:
                GameManager.Instance.SyncDefenderStats();
                break;

            case UpgradeEffectType.BuyTurret:
                GameManager.Instance.SpawnTurret();
                DefenderManager.Instance.AddDefender(DefenderType.Turret);
                break;

            case UpgradeEffectType.BuyMoat:
                GameManager.Instance.SpawnMoat();
                DefenderManager.Instance.AddDefender(DefenderType.Moat);
                GameManager.Instance.SyncDefenderStats();
                UnlockUpgrade(UpgradeEffectType.LavaMoatUnlock);
                break;

            case UpgradeEffectType.LavaMoatUnlock:
                GameManager.Instance.UpgradeMoatToLava();
                GameManager.Instance.SyncDefenderStats();
                UnlockUpgrade(UpgradeEffectType.LavaMoatDps);
                break;

            case UpgradeEffectType.BuyKnight:
                GameManager.Instance.SpawnKnight();
                DefenderManager.Instance.AddDefender(DefenderType.Knight);
                break;
        }
    }

    void UnlockUpgrade(UpgradeEffectType effectType)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].effect.effectType == effectType && upgrades[i].state == UpgradeState.Locked)
            {
                Upgrade u = upgrades[i];
                u.state = UpgradeState.Available;
                upgrades[i] = u;
                break;
            }
        }
    }

    void UnlockNextTier(Upgrade purchasedUpgrade)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade candidate = upgrades[i];
            if (candidate.effect.effectType != UpgradeEffectType.Income)
                continue;

            bool sameTarget =
                candidate.effect.targetResourceType == purchasedUpgrade.effect.targetResourceType;
            bool nextTier = candidate.tier == purchasedUpgrade.tier + 1;

            if (candidate.state == UpgradeState.Locked && sameTarget && nextTier)
            {
                candidate.state = UpgradeState.Available;
                upgrades[i] = candidate;
                break;
            }
        }
    }

    Upgrade GetUpgradeByEffectType(UpgradeEffectType effectType)
    {
        for (int i = 0; i < upgrades.Count; i++)
            if (upgrades[i].effect.effectType == effectType)
                return upgrades[i];
        return null;
    }

    public double GetMoatCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyMoat);
        return u != null ? u.GetCost() : 0;
    }

    public double GetLavaMoatUnlockCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.LavaMoatUnlock);
        return u != null ? u.GetCost() : 0;
    }

    public double moatCost => GetMoatCost();
    public double lavaMoatUnlockCost => GetLavaMoatUnlockCost();
    public double lavaMoatGrowth => LavaMoatDpsGrowth;

    int GetUpgradeIndexByEffectType(UpgradeEffectType effectType)
    {
        for (int i = 0; i < upgrades.Count; i++)
            if (upgrades[i].effect.effectType == effectType)
                return i;
        return -1;
    }

    public int GetNextAvailableUpgradeIndex(ResourceType targetType)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade upgrade = upgrades[i];
            if (upgrade.effect.effectType == UpgradeEffectType.Income &&
                upgrade.effect.targetResourceType == targetType &&
                upgrade.state == UpgradeState.Available)
                return i;
        }
        return -1;
    }

    public Upgrade GetNextAvailableUpgrade(ResourceType targetType)
    {
        int index = GetNextAvailableUpgradeIndex(targetType);

        if (index < 0)
            return null;

        return upgrades[index];
    }

    public double GetCastleDamage()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleDamage);
        return u != null ? 1 + u.level : 1;
    }

    public double GetCastleDamageUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleDamage);
        return u != null ? u.GetCost() : 0;
    }

    public double GetTurretDamage()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.TurretDamage);
        return u != null ? 1 + u.level : 1;
    }

    public double GetTurretDamageUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.TurretDamage);
        return u != null ? u.GetCost() : 0;
    }

    public int GetTurretCount()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyTurret);
        return u != null ? u.level : 0;
    }

    public double GetTurretCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyTurret);
        return u != null ? u.GetCost() : 0;
    }

    public bool IsMoatPurchased()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyMoat);
        return u != null && u.level > 0;
    }

    public bool IsLavaMoatPurchased()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.LavaMoatUnlock);
        return u != null && u.level > 0;
    }

    public bool TryBuyMoat() =>
        TryPurchaseUpgradeByEffectType(UpgradeEffectType.BuyMoat);

    public bool TryUpgradeToLavaMoat() =>
        TryPurchaseUpgradeByEffectType(UpgradeEffectType.LavaMoatUnlock);

    public double GetLavaMoatDps()
    {
        if (!IsLavaMoatPurchased()) return 0;
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.LavaMoatDps);
        return u != null ? LavaMoatDpsBase * Math.Pow(LavaMoatDpsGrowth, u.level) : 0;
    }

    public double GetLavaMoatUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.LavaMoatDps);
        return u != null ? u.GetCost() : 0;
    }

    public int GetKnightCount()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyKnight);
        return u != null ? u.level : 0;
    }

    public double GetKnightDamage() => KnightDamagePerKnight;

    public double GetKnightCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyKnight);
        return u != null ? u.GetCost() : 0;
    }

    public bool TryPurchaseUpgradeByEffectType(UpgradeEffectType effectType)
    {
        int index = GetUpgradeIndexByEffectType(effectType);
        return TryPurchaseUpgrade(index);
    }
}