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

    public Action OnDefendersChanged;

    void Awake()
    {
        Instance = this;

        defendersOwned[DefenderType.CastleCannon] = 1;
        defendersOwned[DefenderType.Turret] = 0;
        defendersOwned[DefenderType.Moat] = 0;
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
}