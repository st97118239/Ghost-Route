using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public static class SaveDataManager
{
    public static SaveData saveData;

    public static void LookForSave()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.txt")) 
            LoadSave();
        else
            CreateSave();
    }

    private static void LoadSave()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/SaveData.txt");
        saveData = JsonUtility.FromJson<SaveData>(json);
        saveData.endings = new();
        for (int i = 0; i < MainMenuManager.endings.Length; i++)
        {
            Ending ending = MainMenuManager.endings[i];
            saveData.endings.Add(ending.endingID, ending);
            ending.isUnlocked = saveData.endingUnlocked[i];
        }

        Debug.Log("Save Loaded");
    }

    private static void CreateSave()
    {
        saveData = new SaveData();
        saveData.Reset(MainMenuManager.endings);
        Save();
    }

    public static void Save()
    {
        if (saveData.endingUnlocked == null || saveData.endingUnlocked.Length == 0) 
            saveData.endingUnlocked = new bool[saveData.endings.Count];

        int idx = 0;
        foreach (KeyValuePair<string, Ending> ending in saveData.endings)
        {
            saveData.endingUnlocked[idx] = ending.Value.isUnlocked;
            idx++;
        }
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.txt", json);
        Debug.Log("Saved");
    }

    public static void ResetData()
    {
        CreateSave();
    }

    public static void ResetGameData()
    {
        saveData.ResetGameSave();
    }
}
