using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public Dictionary<string, float> resources = new Dictionary<string, float>();

    void Awake()
    {
        Instance = this;

        resources["Coins"] = 0f;
        resources["Mana"] = 0f;
    }

    public float GetResource(string resourceName)
    {
        return resources[resourceName];
    }

    public void AddResource(string resourceName, float amount)
    {
        resources[resourceName] += amount;
    }

    public bool SpendResource(string resourceName, float amount)
    {
        if (resources[resourceName] < amount)
            return false;

        resources[resourceName] -= amount;
        return true;
    }

    public void SetResource(string resourceName, float amount)
    {
        resources[resourceName] = amount;
    }
}