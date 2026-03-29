// One live upgrade (per shop button): price curve, level, state, and what it does in-game.
[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    public ResourceType costResourceType;
    public double baseCost;
    public int level;
    public int tier;
    public double growthRate = 1.15;
    public UpgradeState state;
    public UpgradeEffect effect;
    public UpgradeEffectType prerequisiteEffectType;
    public int prerequisiteLevel;

    // Next purchase price: base × (growth ^ current level).
    public double GetCost()
    {
        return baseCost * System.Math.Pow(growthRate, level);
    }
}
