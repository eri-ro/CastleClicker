using System;
using System.Collections.Generic;
using UnityEngine;
// Buys upgrades, tracks levels, merges inspector list with defaults, and applies unlocks/spawns.
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Tooltip("Assign upgrade assets here. Order = runtime order. If empty, default upgrades are created in code.")]
    [SerializeField] List<UpgradeDefinition> upgradeDefinitions = new List<UpgradeDefinition>();

    [HideInInspector]
    public List<Upgrade> upgrades = new List<Upgrade>();

    List<UpgradeDefinition> cachedDefinitions;

    const double LavaMoatDpsBase = 5;
    const double LavaMoatDpsGrowth = 1.25;
    const double MoatMonsterDamageEach = 1;

    void Awake()
    {
        Instance = this;
        cachedDefinitions = ResolveDefinitions();
        BuildUpgradesFromDefinitions(cachedDefinitions);
    }

    public void ResetForNewGame()
    {
        cachedDefinitions = ResolveDefinitions();
        BuildUpgradesFromDefinitions(cachedDefinitions);
    }

    List<UpgradeDefinition> ResolveDefinitions()
    {
        if (upgradeDefinitions == null || upgradeDefinitions.Count == 0)
            return GetDefaultDefinitions();
        return MergeWithDefaultDefinitions(upgradeDefinitions);
    }

    static List<UpgradeDefinition> MergeWithDefaultDefinitions(List<UpgradeDefinition> fromInspector)
    {
        List<UpgradeDefinition> defaults = GetDefaultDefinitions();

        Dictionary<UpgradeEffectType, UpgradeDefinition> byEffect = new Dictionary<UpgradeEffectType, UpgradeDefinition>();
        Dictionary<(ResourceType target, int tier), UpgradeDefinition> incomeByTier =
            new Dictionary<(ResourceType target, int tier), UpgradeDefinition>();

        foreach (UpgradeDefinition def in fromInspector)
        {
            if (def == null) continue;
            if (def.effectType == UpgradeEffectType.Income)
                incomeByTier[(def.targetResourceType, def.tier)] = def;
            else
                byEffect[def.effectType] = def;
        }

        List<UpgradeDefinition> merged = new List<UpgradeDefinition>();

        foreach (UpgradeDefinition def in defaults)
        {
            if (def.effectType == UpgradeEffectType.Income)
            {
                (ResourceType target, int tier) key = (def.targetResourceType, def.tier);
                if (incomeByTier.TryGetValue(key, out UpgradeDefinition userIncome))
                    merged.Add(userIncome);
                else
                    merged.Add(def);
            }
            else if (byEffect.TryGetValue(def.effectType, out UpgradeDefinition userDef))
                merged.Add(userDef);
            else
                merged.Add(def);
        }

        foreach (UpgradeDefinition def in fromInspector)
        {
            if (def == null) continue;
            if (MatchesAnyDefaultSlot(def, defaults))
                continue;
            merged.Add(def);
        }

        return merged;
    }

    static bool MatchesAnyDefaultSlot(UpgradeDefinition candidate, List<UpgradeDefinition> defaults)
    {
        foreach (UpgradeDefinition def in defaults)
        {
            if (def.effectType != candidate.effectType)
                continue;
            if (def.effectType == UpgradeEffectType.Income)
            {
                if (def.targetResourceType == candidate.targetResourceType && def.tier == candidate.tier)
                    return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    void BuildUpgradesFromDefinitions(IEnumerable<UpgradeDefinition> definitions)
    {
        upgrades.Clear();
        foreach (UpgradeDefinition def in definitions)
            upgrades.Add(def.ToUpgrade());
        CheckAndUnlockPrerequisites();
    }

    static List<UpgradeDefinition> GetDefaultDefinitions()
    {
        List<UpgradeDefinition> list = new List<UpgradeDefinition>();

        list.Add(CreateDef("Coin Income I", ResourceType.Coins, 25, 1.25, 1, UpgradeState.Available, UpgradeEffectType.Income, ResourceType.Coins, 1.5));
        list.Add(CreateDef("Mana Income I", ResourceType.Coins, 40, 1.30, 1, UpgradeState.Available, UpgradeEffectType.Income, ResourceType.Mana, 2.0));
        list.Add(CreateDef("Coin Income II", ResourceType.Coins, 100, 1.35, 2, UpgradeState.Locked, UpgradeEffectType.Income, ResourceType.Coins, 2.0));
        list.Add(CreateDef("Mana Income II", ResourceType.Coins, 150, 1.40, 2, UpgradeState.Locked, UpgradeEffectType.Income, ResourceType.Mana, 2.5));
        list.Add(CreateDef("Castle Damage", ResourceType.Coins, 10, 1.15, 0, UpgradeState.Available, UpgradeEffectType.CastleDamage, ResourceType.Coins, 1));
        list.Add(CreateCannonDef("Cannon Fire Rate", ResourceType.Coins, 35, 1.2, UpgradeState.Locked, UpgradeEffectType.CastleCannonFireRate, UpgradeEffectType.CastleDamage, 1));
        list.Add(CreateCannonDef("Cannon Multi-Shot", ResourceType.Coins, 80, 1, UpgradeState.Locked, UpgradeEffectType.CastleCannonMultiShot, UpgradeEffectType.CastleDamage, 2));
        list.Add(CreateCannonDef("Cannon Piercing", ResourceType.Coins, 120, 1, UpgradeState.Locked, UpgradeEffectType.CastleCannonPiercing, UpgradeEffectType.CastleCannonMultiShot, 1));
        list.Add(CreateCannonDef("Cannon Splash", ResourceType.Coins, 60, 1.25, UpgradeState.Locked, UpgradeEffectType.CastleCannonSplash, UpgradeEffectType.CastleDamage, 1));
        list.Add(CreateDef("Turret Damage", ResourceType.Coins, 25, 1.18, 0, UpgradeState.Available, UpgradeEffectType.TurretDamage, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Turret", ResourceType.Coins, 50, 1.25, 0, UpgradeState.Available, UpgradeEffectType.BuyTurret, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Moat", ResourceType.Coins, 150, 1, 0, UpgradeState.Available, UpgradeEffectType.BuyMoat, ResourceType.Coins, 1));
        list.Add(CreateCannonDef("Lava Moat Unlock", ResourceType.Coins, 300, 1, UpgradeState.Locked, UpgradeEffectType.LavaMoatUnlock, UpgradeEffectType.BuyMoat, 1));
        list.Add(CreateDef("Lava Moat DPS", ResourceType.Coins, 100, 1.22, 0, UpgradeState.Locked, UpgradeEffectType.LavaMoatDps, ResourceType.Coins, 1));
        list.Add(CreateDef("Buy Moat Monster", ResourceType.Coins, 75, 1.22, 0, UpgradeState.Available, UpgradeEffectType.BuyMoatMonster, ResourceType.Coins, 1));
        list.Add(CreateMoatMonsterDamageDef());

        return list;
    }

    static UpgradeDefinition CreateDef(string name, ResourceType costType, double baseCost, double growth, int tier, UpgradeState state, UpgradeEffectType effectType, ResourceType targetType, double multiplier)
    {
        UpgradeDefinition def = ScriptableObject.CreateInstance<UpgradeDefinition>();
        def.upgradeName = name;
        def.costResourceType = costType;
        def.baseCost = baseCost;
        def.growthRate = growth;
        def.tier = tier;
        def.initialState = state;
        def.effectType = effectType;
        def.targetResourceType = targetType;
        def.multiplier = multiplier;
        def.prerequisiteLevel = 0;
        return def;
    }

    static UpgradeDefinition CreateMoatMonsterDamageDef()
    {
        UpgradeDefinition def = ScriptableObject.CreateInstance<UpgradeDefinition>();
        def.upgradeName = "Moat Monster Damage";
        def.costResourceType = ResourceType.Coins;
        def.baseCost = 40;
        def.growthRate = 1.18;
        def.tier = 0;
        def.initialState = UpgradeState.Locked;
        def.effectType = UpgradeEffectType.MoatMonsterDamage;
        def.targetResourceType = ResourceType.Coins;
        def.multiplier = 1;
        def.prerequisiteEffectType = UpgradeEffectType.BuyMoatMonster;
        def.prerequisiteLevel = 1;
        return def;
    }

    static UpgradeDefinition CreateCannonDef(string name, ResourceType costType, double baseCost, double growth, UpgradeState state, UpgradeEffectType effectType, UpgradeEffectType prereq, int prereqLevel)
    {
        UpgradeDefinition def = ScriptableObject.CreateInstance<UpgradeDefinition>();
        def.upgradeName = name;
        def.costResourceType = costType;
        def.baseCost = baseCost;
        def.growthRate = growth;
        def.tier = 0;
        def.initialState = state;
        def.effectType = effectType;
        def.targetResourceType = ResourceType.Coins;
        def.multiplier = 1;
        def.prerequisiteEffectType = prereq;
        def.prerequisiteLevel = prereqLevel;
        return def;
    }

    public bool TryPurchaseUpgrade(int index)
    {
        return TryPurchaseUpgrade(index, out _);
    }

    public bool TryPurchaseUpgrade(int index, out string errorMessage)
    {
        errorMessage = null;
        if (index < 0 || index >= upgrades.Count)
        {
            errorMessage = "Invalid upgrade.";
            return false;
        }

        Upgrade upgrade = upgrades[index];
        bool repeatable = IsRepeatable(upgrade);
        bool oneShotPurchased = IsOneShot(upgrade) && upgrade.level > 0;

        if (!repeatable && upgrade.state != UpgradeState.Available)
        {
            errorMessage = "This upgrade is not available yet.";
            return false;
        }
        if (!MeetsPrerequisite(upgrade))
        {
            errorMessage = "Prerequisite upgrade required.";
            return false;
        }
        if (oneShotPurchased)
        {
            errorMessage = "Already purchased.";
            return false;
        }
        if (upgrade.effect.effectType == UpgradeEffectType.BuyTurret && upgrade.level >= 16)
        {
            errorMessage = "Maximum turrets reached.";
            return false;
        }
        if (upgrade.effect.effectType == UpgradeEffectType.BuyMoatMonster && upgrade.level >= GameManager.MaxMoatMonsters)
        {
            errorMessage = "Maximum moat monsters reached.";
            return false;
        }

        double cost = upgrade.GetCost();
        if (upgrade.costResourceType == ResourceType.Coins)
            cost *= LegacyManager.Instance.GetCoinCostMultiplier();
        if (!SpendUpgradeCost(upgrade.costResourceType, cost))
        {
            errorMessage = upgrade.costResourceType == ResourceType.Coins
                ? "Not enough coins."
                : "Not enough mana.";
            return false;
        }

        upgrade.level++;
        ApplyUpgradeEffect(upgrade);

        if (upgrade.effect.effectType == UpgradeEffectType.Income)
        {
            upgrade.state = UpgradeState.Purchased;
            UnlockNextTier(upgrade);
        }
        else if (IsOneShot(upgrade))
            upgrade.state = UpgradeState.Purchased;

        CheckAndUnlockPrerequisites();

        GameManager.Instance.NotifyUpgradeSystemChanged();
        return true;
    }

    static bool IsRepeatable(Upgrade u)
    {
        return u.effect.effectType != UpgradeEffectType.Income
            && u.effect.effectType != UpgradeEffectType.BuyMoat
            && u.effect.effectType != UpgradeEffectType.LavaMoatUnlock
            && u.effect.effectType != UpgradeEffectType.CastleCannonMultiShot
            && u.effect.effectType != UpgradeEffectType.CastleCannonPiercing;
    }

    static bool IsOneShot(Upgrade u)
    {
        return u.effect.effectType == UpgradeEffectType.BuyMoat
            || u.effect.effectType == UpgradeEffectType.LavaMoatUnlock
            || u.effect.effectType == UpgradeEffectType.CastleCannonMultiShot
            || u.effect.effectType == UpgradeEffectType.CastleCannonPiercing;
    }

    bool MeetsPrerequisite(Upgrade u)
    {
        if (u.prerequisiteLevel <= 0) return true;
        Upgrade prereq = GetUpgradeByEffectType(u.prerequisiteEffectType);
        return prereq != null && prereq.level >= u.prerequisiteLevel;
    }

    public bool IsPrerequisiteSatisfied(UpgradeEffectType effectType)
    {
        Upgrade u = GetUpgradeByEffectType(effectType);
        if (u == null) return false;
        return MeetsPrerequisite(u);
    }

    void CheckAndUnlockPrerequisites()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade u = upgrades[i];
            if (u.state != UpgradeState.Locked)
                continue;
            // Tier 2+ Income unlocks only via UnlockNextTier after buying the previous tier, not here.
            if (u.effect.effectType == UpgradeEffectType.Income && u.tier > 1)
                continue;
            if (!MeetsPrerequisite(u))
                continue;
            u.state = UpgradeState.Available;
            upgrades[i] = u;
        }
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
            case UpgradeEffectType.CastleCannonFireRate:
            case UpgradeEffectType.CastleCannonMultiShot:
            case UpgradeEffectType.CastleCannonPiercing:
            case UpgradeEffectType.CastleCannonSplash:
            case UpgradeEffectType.TurretDamage:
            case UpgradeEffectType.LavaMoatDps:
            case UpgradeEffectType.MoatMonsterDamage:
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

            case UpgradeEffectType.BuyMoatMonster:
                GameManager.Instance.SpawnMoatMonster();
                DefenderManager.Instance.AddDefender(DefenderType.MoatMonster);
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

    public Upgrade GetUpgradeByEffectType(UpgradeEffectType effectType)
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
        CheckAndUnlockPrerequisites();
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
        double baseDmg = u != null ? 1 + u.level : 1;
        return ApplyLegacyDefenderMultiplier(baseDmg);
    }

    public double GetCastleDamageUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleDamage);
        return u != null ? u.GetCost() : 0;
    }

    const float CannonBaseFireCooldown = 0.22f;
    const float CannonFireRateReductionPerLevel = 0.12f;

    public float GetCannonFireCooldown()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonFireRate);
        int level = u != null ? u.level : 0;
        float reduction = 1f - level * CannonFireRateReductionPerLevel;
        return Mathf.Max(0.05f, CannonBaseFireCooldown * reduction);
    }

    public bool HasCannonMultiShot()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonMultiShot);
        return u != null && u.level > 0;
    }

    public bool HasCannonPiercing()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonPiercing);
        return u != null && u.level > 0;
    }

    public float GetCannonSplashRadius()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonSplash);
        if (u == null || u.level <= 0) return 0f;
        return 0.5f + u.level * 0.3f;
    }

    public double GetCannonFireRateCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonFireRate);
        return u != null ? u.GetCost() : 0;
    }

    public double GetCannonMultiShotCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonMultiShot);
        return u != null ? u.GetCost() : 0;
    }

    public double GetCannonPiercingCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonPiercing);
        return u != null ? u.GetCost() : 0;
    }

    public double GetCannonSplashCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.CastleCannonSplash);
        return u != null ? u.GetCost() : 0;
    }

    public double GetTurretDamage()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.TurretDamage);
        double baseDmg = u != null ? 1 + u.level : 1;
        return ApplyLegacyDefenderMultiplier(baseDmg);
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
        double baseDps = u != null ? LavaMoatDpsBase * Math.Pow(LavaMoatDpsGrowth, u.level) : 0;
        return ApplyLegacyDefenderMultiplier(baseDps);
    }

    public double GetLavaMoatUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.LavaMoatDps);
        return u != null ? u.GetCost() : 0;
    }

    public int GetMoatMonsterCount()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyMoatMonster);
        return u != null ? u.level : 0;
    }

    public double GetMoatMonsterDamage()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.MoatMonsterDamage);
        double baseDmg = MoatMonsterDamageEach + (u != null ? u.level : 0);
        return ApplyLegacyDefenderMultiplier(baseDmg);
    }

    public double GetMoatMonsterDamageUpgradeCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.MoatMonsterDamage);
        return u != null ? u.GetCost() : 0;
    }

    public double GetMoatMonsterDamageNextPreview()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.MoatMonsterDamage);
        int levelAfterPurchase = (u != null ? u.level : 0) + 1;
        return ApplyLegacyDefenderMultiplier(MoatMonsterDamageEach + levelAfterPurchase);
    }

    static double ApplyLegacyDefenderMultiplier(double value)
    {
        return value * LegacyManager.Instance.GetDefenderDamageMultiplier();
    }

    public double GetMoatMonsterCost()
    {
        Upgrade u = GetUpgradeByEffectType(UpgradeEffectType.BuyMoatMonster);
        return u != null ? u.GetCost() : 0;
    }

    public bool TryPurchaseUpgradeByEffectType(UpgradeEffectType effectType)
    {
        return TryPurchaseUpgradeByEffectType(effectType, out _);
    }

    public bool TryPurchaseUpgradeByEffectType(UpgradeEffectType effectType, out string errorMessage)
    {
        int index = GetUpgradeIndexByEffectType(effectType);
        return TryPurchaseUpgrade(index, out errorMessage);
    }
}
