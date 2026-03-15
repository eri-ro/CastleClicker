using UnityEngine;
using UnityEditor;

public static class CreateDefaultUpgrades
{
    const string Folder = "Assets/Data/Upgrades";

    [MenuItem("Tools/CastleClicker/Create Default Upgrade Definitions")]
    public static void Create()
    {
        EnsureFolderExists("Assets", "Data");
        EnsureFolderExists("Assets/Data", "Upgrades");

        CreateAsset("CoinIncomeI", def =>
        {
            def.upgradeName = "Coin Income I";
            def.costResourceType = ResourceType.Coins;
            def.baseCost = 25;
            def.growthRate = 1.25;
            def.tier = 1;
            def.initialState = UpgradeState.Available;
            def.effectType = UpgradeEffectType.Income;
            def.targetResourceType = ResourceType.Coins;
            def.multiplier = 1.5;
        });

        CreateAsset("ManaIncomeI", def =>
        {
            def.upgradeName = "Mana Income I";
            def.costResourceType = ResourceType.Coins;
            def.baseCost = 40;
            def.growthRate = 1.30;
            def.tier = 1;
            def.initialState = UpgradeState.Available;
            def.effectType = UpgradeEffectType.Income;
            def.targetResourceType = ResourceType.Mana;
            def.multiplier = 2.0;
        });

        CreateAsset("CoinIncomeII", def =>
        {
            def.upgradeName = "Coin Income II";
            def.costResourceType = ResourceType.Coins;
            def.baseCost = 100;
            def.growthRate = 1.35;
            def.tier = 2;
            def.initialState = UpgradeState.Locked;
            def.effectType = UpgradeEffectType.Income;
            def.targetResourceType = ResourceType.Coins;
            def.multiplier = 2.0;
        });

        CreateAsset("ManaIncomeII", def =>
        {
            def.upgradeName = "Mana Income II";
            def.costResourceType = ResourceType.Coins;
            def.baseCost = 150;
            def.growthRate = 1.40;
            def.tier = 2;
            def.initialState = UpgradeState.Locked;
            def.effectType = UpgradeEffectType.Income;
            def.targetResourceType = ResourceType.Mana;
            def.multiplier = 2.5;
        });

        CreateAsset("CastleDamage", def =>
        {
            def.upgradeName = "Castle Damage";
            def.baseCost = 10;
            def.growthRate = 1.15;
            def.effectType = UpgradeEffectType.CastleDamage;
        });

        CreateAsset("TurretDamage", def =>
        {
            def.upgradeName = "Turret Damage";
            def.baseCost = 25;
            def.growthRate = 1.18;
            def.effectType = UpgradeEffectType.TurretDamage;
        });

        CreateAsset("BuyTurret", def =>
        {
            def.upgradeName = "Buy Turret";
            def.baseCost = 50;
            def.growthRate = 1.25;
            def.effectType = UpgradeEffectType.BuyTurret;
        });

        CreateAsset("BuyMoat", def =>
        {
            def.upgradeName = "Buy Moat";
            def.baseCost = 150;
            def.growthRate = 1;
            def.effectType = UpgradeEffectType.BuyMoat;
        });

        CreateAsset("LavaMoatUnlock", def =>
        {
            def.upgradeName = "Lava Moat Unlock";
            def.baseCost = 300;
            def.growthRate = 1;
            def.initialState = UpgradeState.Locked;
            def.effectType = UpgradeEffectType.LavaMoatUnlock;
        });

        CreateAsset("LavaMoatDps", def =>
        {
            def.upgradeName = "Lava Moat DPS";
            def.baseCost = 100;
            def.growthRate = 1.22;
            def.initialState = UpgradeState.Locked;
            def.effectType = UpgradeEffectType.LavaMoatDps;
        });

        CreateAsset("BuyKnight", def =>
        {
            def.upgradeName = "Buy Knight";
            def.baseCost = 75;
            def.growthRate = 1.22;
            def.effectType = UpgradeEffectType.BuyKnight;
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Created default upgrade definitions in {Folder}. Assign them to UpgradeManager's Upgrade Definitions list.");
    }

    static void EnsureFolderExists(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent))
            return;
        string path = $"{parent}/{name}";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    static void CreateAsset(string fileName, System.Action<UpgradeDefinition> setup)
    {
        var def = ScriptableObject.CreateInstance<UpgradeDefinition>();
        setup(def);
        string path = $"{Folder}/{fileName}.asset";
        AssetDatabase.CreateAsset(def, path);
    }
}
