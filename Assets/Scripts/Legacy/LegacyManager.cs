using System;
using UnityEngine;
// Saves mana and meta upgrades between runs; applies bonuses when a new run starts.
public class LegacyManager : MonoBehaviour
{
    public static LegacyManager Instance { get; private set; }

    // PlayerPrefs keys for persistence
    const string PrefsKeyTotalMana = "CastleClicker_TotalMana";
    const string PrefsKeyStartingCoins = "CastleClicker_Legacy_StartingCoins";
    const string PrefsKeyStartingManaPerSec = "CastleClicker_Legacy_StartingManaPerSec";
    const string PrefsKeyStartingCastleHealth = "CastleClicker_Legacy_StartingCastleHealth";
    const string PrefsKeyDefenderDamage = "CastleClicker_Legacy_DefenderDamage";
    const string PrefsKeyStartingCps = "CastleClicker_Legacy_StartingCps";
    const string PrefsKeyEnemyCoinDrop = "CastleClicker_Legacy_EnemyCoinDrop";
    const string PrefsKeyWaveReward = "CastleClicker_Legacy_WaveReward";
    const string PrefsKeyUpgradeDiscount = "CastleClicker_Legacy_UpgradeDiscount";

    public event Action OnLegacyDataChanged;

    [Header("Legacy Upgrade Costs (mana per level)")]
    public double startingCoinsCostPerLevel = 50;
    public double startingManaPerSecCostPerLevel = 100;
    public double startingCastleHealthCostPerLevel = 75;
    public double defenderDamageCostPerLevel = 250;
    public double startingCpsCostPerLevel = 350;
    public double enemyCoinDropCostPerLevel = 400;
    public double waveRewardCostPerLevel = 450;
    public double upgradeDiscountCostPerLevel = 500;

    [Header("Legacy Upgrade Bonuses per level")]
    public double startingCoinsBonusPerLevel = 25;
    public double startingManaPerSecBonusPerLevel = 0.5;
    public int startingCastleHealthBonusPerLevel = 25;
    public double defenderDamageBonusPerLevel = 0.15;
    public double startingCpsBonusPerLevel = 0.5;
    public double enemyCoinDropBonusPerLevel = 0.25;
    public double waveRewardBonusPerLevel = 0.30;
    public double upgradeDiscountPerLevel = 0.10;

    double totalMana;
    int legacyStartingCoinsLevel;
    int legacyStartingManaPerSecLevel;
    int legacyStartingCastleHealthLevel;
    int legacyDefenderDamageLevel;
    int legacyStartingCpsLevel;
    int legacyEnemyCoinDropLevel;
    int legacyWaveRewardLevel;
    int legacyUpgradeDiscountLevel;

    public double TotalMana => totalMana;
    public int LegacyStartingCoinsLevel => legacyStartingCoinsLevel;
    public int LegacyStartingManaPerSecLevel => legacyStartingManaPerSecLevel;
    public int LegacyStartingCastleHealthLevel => legacyStartingCastleHealthLevel;
    public int LegacyDefenderDamageLevel => legacyDefenderDamageLevel;
    public int LegacyStartingCpsLevel => legacyStartingCpsLevel;
    public int LegacyEnemyCoinDropLevel => legacyEnemyCoinDropLevel;
    public int LegacyWaveRewardLevel => legacyWaveRewardLevel;
    public int LegacyUpgradeDiscountLevel => legacyUpgradeDiscountLevel;

    void Awake()
    {
        Instance = this;
        Load();
    }

    void Start()
    {
        CastleManager.Instance.OnGameOver += OnGameOver;
    }

    void OnDestroy()
    {
        CastleManager.Instance.OnGameOver -= OnGameOver;
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Save();
    }

    void OnGameOver()
    {
        BankRunMana();
    }

    public void BankRunMana()
    {
        double manaEarned = ResourceManager.Instance.GetResource(ResourceType.Mana);
        AddMana(manaEarned);
    }

    void Load()
    {
        totalMana = GetDoublePref(PrefsKeyTotalMana, 0);
        legacyStartingCoinsLevel = PlayerPrefs.GetInt(PrefsKeyStartingCoins, 0);
        legacyStartingManaPerSecLevel = PlayerPrefs.GetInt(PrefsKeyStartingManaPerSec, 0);
        legacyStartingCastleHealthLevel = PlayerPrefs.GetInt(PrefsKeyStartingCastleHealth, 0);
        legacyDefenderDamageLevel = PlayerPrefs.GetInt(PrefsKeyDefenderDamage, 0);
        legacyStartingCpsLevel = PlayerPrefs.GetInt(PrefsKeyStartingCps, 0);
        legacyEnemyCoinDropLevel = PlayerPrefs.GetInt(PrefsKeyEnemyCoinDrop, 0);
        legacyWaveRewardLevel = PlayerPrefs.GetInt(PrefsKeyWaveReward, 0);
        legacyUpgradeDiscountLevel = PlayerPrefs.GetInt(PrefsKeyUpgradeDiscount, 0);
    }

    void Save()
    {
        SetDoublePref(PrefsKeyTotalMana, totalMana);
        PlayerPrefs.SetInt(PrefsKeyStartingCoins, legacyStartingCoinsLevel);
        PlayerPrefs.SetInt(PrefsKeyStartingManaPerSec, legacyStartingManaPerSecLevel);
        PlayerPrefs.SetInt(PrefsKeyStartingCastleHealth, legacyStartingCastleHealthLevel);
        PlayerPrefs.SetInt(PrefsKeyDefenderDamage, legacyDefenderDamageLevel);
        PlayerPrefs.SetInt(PrefsKeyStartingCps, legacyStartingCpsLevel);
        PlayerPrefs.SetInt(PrefsKeyEnemyCoinDrop, legacyEnemyCoinDropLevel);
        PlayerPrefs.SetInt(PrefsKeyWaveReward, legacyWaveRewardLevel);
        PlayerPrefs.SetInt(PrefsKeyUpgradeDiscount, legacyUpgradeDiscountLevel);
        PlayerPrefs.Save();
    }

    static double GetDoublePref(string key, double defaultValue)
    {
        return PlayerPrefs.HasKey(key) ? double.Parse(PlayerPrefs.GetString(key)) : defaultValue;
    }

    static void SetDoublePref(string key, double value)
    {
        PlayerPrefs.SetString(key, value.ToString("R"));
    }

    void AddMana(double amount)
    {
        if (amount <= 0) return;
        totalMana += amount;
        Save();
        OnLegacyDataChanged?.Invoke();
    }

    public bool SpendMana(double amount)
    {
        if (amount <= 0 || totalMana < amount) return false;
        totalMana -= amount;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public double GetStartingCoinsBonus() => legacyStartingCoinsLevel * startingCoinsBonusPerLevel;
    public double GetStartingManaPerSecBonus() => legacyStartingManaPerSecLevel * startingManaPerSecBonusPerLevel;
    public int GetStartingCastleHealthBonus() => legacyStartingCastleHealthLevel * startingCastleHealthBonusPerLevel;

    public double GetStartingCoinsCost() => (legacyStartingCoinsLevel + 1) * startingCoinsCostPerLevel;
    public double GetStartingManaPerSecCost() => (legacyStartingManaPerSecLevel + 1) * startingManaPerSecCostPerLevel;
    public double GetStartingCastleHealthCost() => (legacyStartingCastleHealthLevel + 1) * startingCastleHealthCostPerLevel;
    public double GetDefenderDamageCost() => (legacyDefenderDamageLevel + 1) * defenderDamageCostPerLevel;
    public double GetStartingCpsCost() => (legacyStartingCpsLevel + 1) * startingCpsCostPerLevel;
    public double GetEnemyCoinDropCost() => (legacyEnemyCoinDropLevel + 1) * enemyCoinDropCostPerLevel;
    public double GetWaveRewardCost() => (legacyWaveRewardLevel + 1) * waveRewardCostPerLevel;
    public double GetUpgradeDiscountCost() => (legacyUpgradeDiscountLevel + 1) * upgradeDiscountCostPerLevel;

    public double GetDefenderDamageMultiplier() => 1.0 + legacyDefenderDamageLevel * defenderDamageBonusPerLevel;
    public double GetStartingCpsBonus() => legacyStartingCpsLevel * startingCpsBonusPerLevel;
    public double GetEnemyCoinDropMultiplier() => 1.0 + legacyEnemyCoinDropLevel * enemyCoinDropBonusPerLevel;
    public double GetWaveRewardMultiplier() => 1.0 + legacyWaveRewardLevel * waveRewardBonusPerLevel;
    public double GetCoinCostMultiplier() => Math.Max(0.5, 1.0 - legacyUpgradeDiscountLevel * upgradeDiscountPerLevel);

    public bool TryBuyStartingCoins()
    {
        double cost = GetStartingCoinsCost();
        if (!SpendMana(cost)) return false;
        legacyStartingCoinsLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyStartingManaPerSec()
    {
        double cost = GetStartingManaPerSecCost();
        if (!SpendMana(cost)) return false;
        legacyStartingManaPerSecLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyStartingCastleHealth()
    {
        double cost = GetStartingCastleHealthCost();
        if (!SpendMana(cost)) return false;
        legacyStartingCastleHealthLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyDefenderDamage()
    {
        double cost = GetDefenderDamageCost();
        if (!SpendMana(cost)) return false;
        legacyDefenderDamageLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyStartingCps()
    {
        double cost = GetStartingCpsCost();
        if (!SpendMana(cost)) return false;
        legacyStartingCpsLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyEnemyCoinDrop()
    {
        double cost = GetEnemyCoinDropCost();
        if (!SpendMana(cost)) return false;
        legacyEnemyCoinDropLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyWaveReward()
    {
        double cost = GetWaveRewardCost();
        if (!SpendMana(cost)) return false;
        legacyWaveRewardLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }

    public bool TryBuyUpgradeDiscount()
    {
        double cost = GetUpgradeDiscountCost();
        if (!SpendMana(cost)) return false;
        legacyUpgradeDiscountLevel++;
        Save();
        OnLegacyDataChanged?.Invoke();
        return true;
    }
}
