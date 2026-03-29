// All upgrade kinds (income, cannon perks, buy turret, moat, etc.).
[System.Serializable]
public enum UpgradeEffectType
{
    Income,
    CastleDamage,
    CastleCannonFireRate,
    CastleCannonMultiShot,
    CastleCannonPiercing,
    CastleCannonSplash,
    TurretDamage,
    BuyTurret,
    BuyMoat,
    LavaMoatUnlock,
    LavaMoatDps,
    BuyMoatMonster,
    MoatMonsterDamage
}

[System.Serializable]
public struct UpgradeEffect
{
    public UpgradeEffectType effectType;
    public double multiplier;
    public ResourceType targetResourceType;
}
