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

    public double GetCost()
    {
        return baseCost * System.Math.Pow(growthRate, level);
    }
}