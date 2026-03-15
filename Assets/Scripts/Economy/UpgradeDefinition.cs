using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "CastleClicker/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;

    [Header("Cost")]
    public ResourceType costResourceType = ResourceType.Coins;
    public double baseCost = 10;
    public double growthRate = 1.15;

    [Header("Tier & State (Income upgrades)")]
    public int tier;
    public UpgradeState initialState = UpgradeState.Available;

    [Header("Effect")]
    public UpgradeEffectType effectType = UpgradeEffectType.CastleDamage;

    [Header("Income effect only")]
    public ResourceType targetResourceType = ResourceType.Coins;
    public double multiplier = 1.5;

    public Upgrade ToUpgrade()
    {
        return new Upgrade
        {
            upgradeName = upgradeName,
            costResourceType = costResourceType,
            baseCost = baseCost,
            level = 0,
            tier = tier,
            growthRate = growthRate,
            state = initialState,
            effect = new UpgradeEffect
            {
                effectType = effectType,
                targetResourceType = targetResourceType,
                multiplier = multiplier
            }
        };
    }
}
