using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public event Action<ResourceType, double> OnResourceChanged;

    private Dictionary<ResourceType, double> resources = new Dictionary<ResourceType, double>();

    private void Awake()
    {
        Instance = this;
        InitializeResources();
    }

    private void InitializeResources()
    {
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0.0;
        }
    }

    public void AddResource(ResourceType resourceType, double amount)
    {
        if (amount <= 0.0)
        {
            return;
        }

        resources[resourceType] += amount;
        RaiseResourceChanged(resourceType);
    }

    public bool SpendResource(ResourceType resourceType, double amount)
    {
        if (amount <= 0.0)
        {
            return false;
        }

        if (resources[resourceType] < amount)
        {
            return false;
        }

        resources[resourceType] -= amount;
        RaiseResourceChanged(resourceType);
        return true;
    }

    public double GetResource(ResourceType resourceType)
    {
        return resources[resourceType];
    }

    public void SetResource(ResourceType resourceType, double amount)
    {
        if (amount < 0.0)
        {
            amount = 0.0;
        }

        resources[resourceType] = amount;
        RaiseResourceChanged(resourceType);
    }

    private void RaiseResourceChanged(ResourceType resourceType)
    {
        OnResourceChanged.Invoke(resourceType, resources[resourceType]);
    }
}