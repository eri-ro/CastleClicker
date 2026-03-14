using System;
using System.Collections.Generic;
using UnityEngine;

public class DefenderManager : MonoBehaviour
{
    public static DefenderManager Instance { get; private set; }

    public Dictionary<DefenderType, int> defendersOwned = new Dictionary<DefenderType, int>();
    public Dictionary<DefenderType, double> defenderDamage = new Dictionary<DefenderType, double>();

    public Action OnDefendersChanged;

    void Awake()
    {
        Instance = this;

        foreach (DefenderType type in System.Enum.GetValues(typeof(DefenderType)))
        {
            defendersOwned[type] = 0;
            defenderDamage[type] = 0.0;
        }
    }

    public void AddDefender(DefenderType type, int amount = 1)
    {
        defendersOwned[type] += amount;
        OnDefendersChanged?.Invoke();
    }

    public void SetDefenderCount(DefenderType type, int amount)
    {
        defendersOwned[type] = amount;
        OnDefendersChanged?.Invoke();
    }

    public int GetCount(DefenderType type)
    {
        return defendersOwned[type];
    }

    public void SetDamage(DefenderType type, double amount)
    {
        defenderDamage[type] = amount;
    }

    public double GetDamage(DefenderType type)
    {
        return defenderDamage[type];
    }
}