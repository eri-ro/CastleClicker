using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> upgrades = new List<Upgrade>();

    void Start()
    {
        upgrades.Add(new Upgrade
        {
            upgradeName = "Castle Damage",
            resourceType = "Coins",
            baseCost = 10f,
            level = 0,
            growthRate = 1.15f
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Turret Damage",
            resourceType = "Coins",
            baseCost = 25f,
            level = 0,
            growthRate = 1.18f
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Mana Generator",
            resourceType = "Coins",
            baseCost = 50f,
            level = 0,
            growthRate = 1.20f
        });

        upgrades.Add(new Upgrade
        {
            upgradeName = "Lava Moat Damage",
            resourceType = "Coins",
            baseCost = 100f,
            level = 0,
            growthRate = 1.22f
        });
    }
}