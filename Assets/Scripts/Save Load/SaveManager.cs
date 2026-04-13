using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private void Awake()
    {
        jsonPath = Path.Combine(Application.persistentDataPath, "playerData.json");
        instance = this;
    }

    public string fileName = "Runtime.txt";
    float playTime = 0;

    string jsonPath;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        SaveJson();
        SaveToFile();
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

    public PlayerData LoadJson()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.Log("No save file found");
            return null;
        }

        string json = File.ReadAllText(jsonPath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        ResourceManager.Instance.SetCoinsPerSecond(data.coinsPerSecond);
        ResourceManager.Instance.cpsLevel = data.cpsLevel;
        ResourceManager.Instance.cpsBaseCost = data.cpsBaseCost;
        ResourceManager.Instance.cpsCostGrowth = data.cpsCostGrowth;
        ResourceManager.Instance.cpsGainPerLevel = data.cpsGainPerLevel;
        ResourceManager.Instance.SetManaPerSecond(data.manaPerSecond);

        Debug.Log("Loaded from: " + jsonPath);
        return data;
    }
}
