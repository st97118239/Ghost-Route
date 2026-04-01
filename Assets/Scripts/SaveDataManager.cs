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

        try
        {
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            Debug.LogError("Save file could not be loaded");
            CreateSave();
            return;
        }

        if (saveData == null)
        {
            Debug.LogError("Save file could not be loaded");
            CreateSave();
            return;
        }

        saveData.endings = new();
        if (MainMenuManager.endings != null && MainMenuManager.endings.Length > 0)
        {
            for (int i = 0; i < MainMenuManager.endings.Length; i++)
            {
                Ending ending = MainMenuManager.endings[i];
                saveData.endings.Add(ending.endingID, ending);
                ending.isUnlocked = saveData.endingUnlocked[i];
            }
        }

        saveData.hasSeenAcheronDialogue = false;

#if UNITY_EDITOR
        Debug.Log("Save loaded");
#endif
    }

    private static void CreateSave()
    {
#if UNITY_EDITOR
        Debug.Log("Creating new save data");
#endif
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
        try
        {
            File.WriteAllText(Application.persistentDataPath + "/SaveData.txt", json);
        }
        catch
        {
            Debug.LogError("Save data could not be saved");
            return;
        }
#if UNITY_EDITOR
        Debug.Log("Successfully saved data");
#endif
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
