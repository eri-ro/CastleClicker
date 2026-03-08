using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    public string resourceType;
    public float baseCost;
    public int level;
    public float growthRate = 1.15f;

    public float GetCost()
    {
        return baseCost * Mathf.Pow(growthRate, level);
    }
}