using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }

    private string dataPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dataPath = Application.persistentDataPath + "/GameData.json";
    }

    public void SaveGame(int levelToSave)
    {
        GameData data = new GameData
        {
            level = levelToSave,
        };

        string json = JsonUtility.ToJson(data, true);
        
        File.WriteAllText(dataPath, json);
    }

    public int LoadLevel()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            return data.level;
        }
        return 0;
    }
}
