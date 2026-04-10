using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public string fileName = "Runtime.txt";
    float playTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //LoadFromFile();
    }

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
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

    public void LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            float loadedTime = 0;
            if (!content.Equals(""))
            {
                loadedTime = float.Parse(content);
            }

            Debug.Log("Loaded time: " + loadedTime);
        }
        else
        {
            Debug.Log("No save file found");
        }
    }
}
