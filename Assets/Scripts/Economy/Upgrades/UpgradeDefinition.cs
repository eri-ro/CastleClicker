using UnityEngine;

// Saved upgrade data (asset). The game turns this into a live Upgrade when the run loads.
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

    [Header("Prerequisite (for tech tree - leave empty if none)")]
    [Tooltip("Upgrade that must be purchased first. E.g. CastleCannonFireRate requires CastleDamage level 1.")]
    public UpgradeEffectType prerequisiteEffectType;
    [Tooltip("Minimum level of prerequisite to unlock this upgrade. 0 = no prerequisite.")]
    public int prerequisiteLevel = 0;

    [Header("Effect")]
    public UpgradeEffectType effectType = UpgradeEffectType.CastleDamage;

    [Header("Income effect only")]
    public ResourceType targetResourceType = ResourceType.Coins;
    public double multiplier = 1.5;

    // Build a fresh level-0 upgrade for the shop list from this asset.
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
            },
            prerequisiteEffectType = prerequisiteEffectType,
            prerequisiteLevel = prerequisiteLevel
        };
    }
}
