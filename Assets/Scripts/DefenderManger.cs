using System;
using System.Collections.Generic;
using UnityEngine;

public enum DefenderType
{
    CastleCannon,
    Turret,
    Moat
}

public class DefenderManager : MonoBehaviour
{
    public static DefenderManager Instance { get; private set; }

    public Dictionary<DefenderType, int> defendersOwned = new Dictionary<DefenderType, int>();
    public Dictionary<DefenderType, double> defenderDamage = new Dictionary<DefenderType, double>();

    public Action OnDefendersChanged;

    void Awake()
    {
        Instance = this;

        defendersOwned[DefenderType.CastleCannon] = 1;
        defendersOwned[DefenderType.Turret] = 0;
        defendersOwned[DefenderType.Moat] = 0;

        defenderDamage[DefenderType.CastleCannon] = 1;
        defenderDamage[DefenderType.Turret] = 1;
        defenderDamage[DefenderType.Moat] = 0;
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
        OnDefendersChanged?.Invoke();
    }

    public double GetDamage(DefenderType type)
    {
        return defenderDamage[type];
    }
}