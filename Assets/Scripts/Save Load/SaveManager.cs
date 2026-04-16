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
            Debug.LogError($"Save Manager was unable to save due to: {e.Message} {e.StackTrace}");
        }
    }

    public PlayerData LoadJson()
    {
        try
        {
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
        catch (Exception e)
        {
            if (e.Message.Contains("does not exist")) // Means save file doesn't exist yet because the player hasn't saved a game yet
            {
                Debug.Log("No existing file to load from!");
                return null;
            }
            else
            {
                throw new ArgumentException($"Save Manager unable to load due to {e.Message} {e.StackTrace}");
            }
        }

    }
}
