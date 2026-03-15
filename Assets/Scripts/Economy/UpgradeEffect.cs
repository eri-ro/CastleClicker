[System.Serializable]
public enum UpgradeEffectType
{
    Income,
    CastleDamage,
    TurretDamage,
    BuyTurret,
    BuyMoat,
    LavaMoatUnlock,
    LavaMoatDps,
    BuyKnight
}

[System.Serializable]
public struct UpgradeEffect
{
    public UpgradeEffectType effectType;
    public double multiplier;
    public ResourceType targetResourceType;
}