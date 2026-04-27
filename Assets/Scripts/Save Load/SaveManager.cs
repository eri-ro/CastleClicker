using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    //week 14 event
    public event Action<string> OnSaveFailed;

    private void Awake()
    {
        jsonPath = Path.Combine(Application.persistentDataPath, "playerData.json");
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public string fileName = "Runtime.txt";
    float playTime = 0;

    string jsonPath;

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        try
        {
            SaveJson();
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveManager: JSON save on quit failed — {e.Message}");
        }

        try
        {
            SaveToFile();
        }
        catch (Exception e)
        {
            OnSaveFailed?.Invoke($"Session log append failed: {e.Message}");
            Debug.LogError($"SaveManager: session log append failed — {e.Message}");
        }
    }

    public void SaveToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        int sessionTime = (int)(playTime += 0.5f);

        string line = $"Session time: {sessionTime}";
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }

        Debug.Log("Appended to " + path);
    }

    public void SaveJson()
    {
        try
        {
            PlayerData data = new PlayerData();
            data.coinsPerSecond = ResourceManager.Instance.coinsPerSecond;
            data.cpsLevel = ResourceManager.Instance.cpsLevel;
            data.cpsBaseCost = ResourceManager.Instance.cpsBaseCost;
            data.cpsCostGrowth = ResourceManager.Instance.cpsCostGrowth;
            data.cpsGainPerLevel = ResourceManager.Instance.cpsGainPerLevel;
            data.manaPerSecond = ResourceManager.Instance.manaPerSecond;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(jsonPath, json);
            Debug.Log("Saved to: " + jsonPath);
        }
        catch (Exception e)
        {
            OnSaveFailed?.Invoke($"SaveJson failed: {e.Message}");
            throw;
        }
    }

    public PlayerData LoadJson()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.Log("No save file found");
            return null;
        }

        try
        {
            string json = File.ReadAllText(jsonPath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            if (data == null)
                throw new InvalidOperationException("playerData.json parsed to null (corrupt or empty).");

            ResourceManager.Instance.SetCoinsPerSecond(data.coinsPerSecond);
            ResourceManager.Instance.cpsLevel = data.cpsLevel;
            ResourceManager.Instance.cpsBaseCost = data.cpsBaseCost;
            ResourceManager.Instance.cpsCostGrowth = data.cpsCostGrowth;
            ResourceManager.Instance.cpsGainPerLevel = data.cpsGainPerLevel;
            ResourceManager.Instance.SetManaPerSecond(data.manaPerSecond);

            Debug.Log("Loaded from: " + jsonPath);
            return data;
        }
        catch (Exception e)
        {
            OnSaveFailed?.Invoke($"LoadJson failed: {e.Message}");
            throw;
        }
    }
}