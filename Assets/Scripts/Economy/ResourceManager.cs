using System;
using System.Collections.Generic;
using UnityEngine;
// Handles coins, mana, and passive income each second
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public event Action<ResourceType, double> OnResourceChanged;
    public event Action OnResourcePerSecondChanged;

    private Dictionary<ResourceType, double> resources = new Dictionary<ResourceType, double>();

    [Header("Coins per second")]
    public double coinsPerSecond = 1;
    public int cpsLevel = 0;
    public double cpsBaseCost = 20;
    public double cpsCostGrowth = 1.17;
    public double cpsGainPerLevel = 1;

    [Header("Mana per second")]
    public double manaPerSecond = 1;

    float passiveResourceTimer;

    readonly ResourceGenerator[] passiveGenerators =
    {
        new GoldMineGenerator(),
        new ManaWellGenerator()
    };

    private void Awake()
    {
        Instance = this;
        InitializeResources();
    }

    private void InitializeResources()
    {
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0.0;
        }
    }

    void Start()
    {
        ApplyLegacyBonuses();
    }

    void ApplyLegacyBonuses()
    {
        LegacyManager lm = LegacyManager.Instance;
        AddResource(ResourceType.Coins, lm.GetStartingCoinsBonus());
        manaPerSecond += lm.GetStartingManaPerSecBonus();
        coinsPerSecond += lm.GetStartingCpsBonus();
        OnResourcePerSecondChanged?.Invoke();
    }

    void Update()
    {
        if (!GameManager.Instance.gameStarted)
            return;
        if (CastleManager.Instance.gameOver)
            return;

        passiveResourceTimer += Time.deltaTime;
        if (passiveResourceTimer >= 1f)
        {
            passiveResourceTimer -= 1f;
            double coinTick = 0;
            double manaTick = 0;
            for (int i = 0; i < passiveGenerators.Length; i++)
                passiveGenerators[i].Produce(ref coinTick, ref manaTick);
            AddResource(ResourceType.Coins, coinTick);
            AddResource(ResourceType.Mana, manaTick);
        }
    }

    public void AddResource(ResourceType resourceType, double amount)
    {
        if (amount <= 0.0)
        {
            return;
        }

        resources[resourceType] += amount;
        RaiseResourceChanged(resourceType);
    }

    public bool SpendResource(ResourceType resourceType, double amount)
    {
        if (amount <= 0.0)
        {
            return false;
        }

        if (resources[resourceType] < amount)
        {
            return false;
        }

        resources[resourceType] -= amount;
        if (resourceType == ResourceType.Coins)
            GameStatsTracker.Instance.RecordCoinsSpent(amount);
        RaiseResourceChanged(resourceType);
        return true;
    }

    public double GetResource(ResourceType resourceType)
    {
        return resources[resourceType];
    }

    private void RaiseResourceChanged(ResourceType resourceType)
    {
        OnResourceChanged?.Invoke(resourceType, resources[resourceType]);
    }

    public void AddCoins(double amount)
    {
        AddResource(ResourceType.Coins, amount);
    }

    public bool SpendCoins(double cost)
    {
        return SpendResource(ResourceType.Coins, cost);
    }

    public double GetCoinsPerSecond()
    {
        return coinsPerSecond;
    }

    public void SetCoinsPerSecond(double value)
    {
        coinsPerSecond = value;
        OnResourcePerSecondChanged?.Invoke();
    }

    public double GetCpsUpgradeCost()
    {
        return cpsBaseCost * Math.Pow(cpsCostGrowth, cpsLevel);
    }

    public double GetCpsGainPerLevel()
    {
        return cpsGainPerLevel;
    }

    public bool TryBuyCpsUpgrade()
    {
        double cost = GetCpsUpgradeCost();
        if (!SpendCoins(cost))
            return false;

        cpsLevel++;
        coinsPerSecond += cpsGainPerLevel;
        OnResourcePerSecondChanged?.Invoke();
        return true;
    }

    public double GetManaPerSecond()
    {
        return manaPerSecond;
    }

    public void SetManaPerSecond(double value)
    {
        manaPerSecond = value;
        OnResourcePerSecondChanged?.Invoke();
    }

    public void ResetForNewGame()
    {
        resources[ResourceType.Coins] = 0;
        resources[ResourceType.Mana] = 0;
        coinsPerSecond = 1;
        cpsLevel = 0;
        manaPerSecond = 1;

        LegacyManager lm = LegacyManager.Instance;
        AddResource(ResourceType.Coins, lm.GetStartingCoinsBonus());
        manaPerSecond += lm.GetStartingManaPerSecBonus();
        coinsPerSecond += lm.GetStartingCpsBonus();

        RaiseResourceChanged(ResourceType.Coins);
        RaiseResourceChanged(ResourceType.Mana);
        OnResourcePerSecondChanged?.Invoke();
    }
}
