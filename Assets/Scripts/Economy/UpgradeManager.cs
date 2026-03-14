using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public List<Upgrade> upgrades = new List<Upgrade>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateStartingUpgrades();
    }

    void CreateStartingUpgrades()
    {
        upgrades.Add(new Upgrade
        {
            upgradeName = "Coin Income I",
            costResourceType = ResourceType.Coins,
            baseCost = 25,
            level = 0,
            tier = 1,
            growthRate = 1.25,
            state = UpgradeState.Available,
            effect = new UpgradeEffect
            {
                multiplier = 1.5,
                targetResourceType = ResourceType.Coins
            }
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Mana Income I",
            costResourceType = ResourceType.Coins,
            baseCost = 40,
            level = 0,
            tier = 1,
            growthRate = 1.30,
            state = UpgradeState.Available,
            effect = new UpgradeEffect
            {
                multiplier = 2.0,
                targetResourceType = ResourceType.Mana
            }
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Coin Income II",
            costResourceType = ResourceType.Coins,
            baseCost = 100,
            level = 0,
            tier = 2,
            growthRate = 1.35,
            state = UpgradeState.Locked,
            effect = new UpgradeEffect
            {
                multiplier = 2.0,
                targetResourceType = ResourceType.Coins
            }
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Mana Income II",
            costResourceType = ResourceType.Coins,
            baseCost = 150,
            level = 0,
            tier = 2,
            growthRate = 1.40,
            state = UpgradeState.Locked,
            effect = new UpgradeEffect
            {
                multiplier = 2.5,
                targetResourceType = ResourceType.Mana
            }
        });
    }

    public bool TryPurchaseUpgrade(int index)
    {
        if (index < 0 || index >= upgrades.Count)
            return false;

        Upgrade upgrade = upgrades[index];

        if (upgrade.state != UpgradeState.Available)
            return false;

        double cost = upgrade.GetCost();

        if (!SpendUpgradeCost(upgrade.costResourceType, cost))
            return false;

        ApplyUpgradeEffect(upgrade);

        upgrade.level++;
        upgrade.state = UpgradeState.Purchased;

        UnlockNextTier(upgrade);

        GameManager.Instance.NotifyUpgradeSystemChanged();
        return true;
    }

    bool SpendUpgradeCost(ResourceType resourceType, double amount)
    {
        switch (resourceType)
        {
            case ResourceType.Coins:
                return GameManager.Instance.SpendCoins(amount);

            case ResourceType.Mana:
                return ResourceManager.Instance.SpendResource(ResourceType.Mana, amount);

            default:
                return false;
        }
    }

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        switch (upgrade.effect.targetResourceType)
        {
            case ResourceType.Coins:
                GameManager.Instance.coinsPerSecond *= upgrade.effect.multiplier;
                break;

            case ResourceType.Mana:
                GameManager.Instance.manaPerSecond *= upgrade.effect.multiplier;
                break;
        }
    }

    void UnlockNextTier(Upgrade purchasedUpgrade)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade candidate = upgrades[i];

            bool sameTarget =
                candidate.effect.targetResourceType == purchasedUpgrade.effect.targetResourceType;

            bool nextTier =
                candidate.tier == purchasedUpgrade.tier + 1;

            if (candidate.state == UpgradeState.Locked && sameTarget && nextTier)
            {
                candidate.state = UpgradeState.Available;
                break;
            }
        }
    }

    public int GetNextAvailableUpgradeIndex(ResourceType targetType)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade upgrade = upgrades[i];

            if (upgrade.effect.targetResourceType == targetType &&
                upgrade.state == UpgradeState.Available)
            {
                return i;
            }
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
}