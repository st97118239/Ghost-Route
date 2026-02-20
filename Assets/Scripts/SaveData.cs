using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveData
{
    public static bool hasSave;
    public static float textSpeed;
    public static int windowType;
    public static float bgmVolume;
    public static float sfxVolume;
    public static float voicelinesVolume;
    public static bool showGore;

    public static string name;
    public static string pronouns;

    public static Dictionary<string, Ending> endings;

    public static string currentDialogueID;

    public static bool hasPlayedGhostHunt;
    public static int ghostHuntScore;

    public static bool hasPlayedAcheron;

    public static bool hasPlayedMemory;
    public static int memoryScore;

    public static void LoadSave()
    {
        hasSave = PlayerPrefs.GetInt("HasSave") != 0;

        if (hasSave) 
            SetData();
        else
            ResetData();
    }

    public static void SetData()
    {
        textSpeed = PlayerPrefs.GetFloat("TextSpeed");
        windowType = PlayerPrefs.GetInt("WindowType");
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        voicelinesVolume = PlayerPrefs.GetFloat("VoicelinesVolume");
        showGore = PlayerPrefs.GetInt("ShowGore") != 0;

        name = PlayerPrefs.GetString("Name");
        pronouns = PlayerPrefs.GetString("Pronouns");

        currentDialogueID = PlayerPrefs.GetString("DialogueID");

        if (endings == null) 
            CreateEndings();

        foreach ((string id, Ending value) in endings) 
            value.isUnlocked = PlayerPrefs.GetInt(id) != 0;

        hasPlayedGhostHunt = PlayerPrefs.GetInt("HasPlayedGhostHunt") != 0;
        ghostHuntScore = PlayerPrefs.GetInt("GhostHuntScore");
        hasPlayedAcheron = PlayerPrefs.GetInt("HasPlayedAcheron") != 0;
        hasPlayedMemory = PlayerPrefs.GetInt("HasPlayedMemory") != 0;
        memoryScore = PlayerPrefs.GetInt("MemoryScore");
    }

    private static void CreateEndings()
    {
        if (MainMenuManager.endings == null)
        {
            SceneManager.LoadScene("Main Menu");
            return;
        }

        endings = new();
        foreach (Ending ending in MainMenuManager.endings)
        {
            endings.Add(ending.endingID, ending);
            ending.isUnlocked = PlayerPrefs.GetInt(ending.endingID) != 0;
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetFloat("TextSpeed", textSpeed);
        PlayerPrefs.SetFloat("WindowType", windowType);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("VoicelinesVolume", voicelinesVolume);
        PlayerPrefs.SetInt("ShowGore", showGore ? 1 : 0);

        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetString("Pronouns", pronouns);

        PlayerPrefs.SetString("DialogueID", currentDialogueID);

        if (endings == null) CreateEndings();

        foreach (KeyValuePair<string, Ending> ending in endings) 
            PlayerPrefs.SetInt(ending.Key, ending.Value.isUnlocked ? 1 : 0);

        PlayerPrefs.SetInt("HasPlayedGhostHunt", hasPlayedGhostHunt ? 1 : 0);
        PlayerPrefs.SetInt("GhostHuntScore", ghostHuntScore);
        PlayerPrefs.SetInt("HasPlayedAcheron", hasPlayedAcheron ? 1 : 0);
        PlayerPrefs.SetInt("HasPlayedMemory", hasPlayedMemory ? 1 : 0);
        PlayerPrefs.SetInt("MemoryScore", memoryScore);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.SetFloat("TextSpeed", 0.075f);
        PlayerPrefs.SetInt("WindowType", 1);
        PlayerPrefs.SetFloat("BGMVolume", 1);
        PlayerPrefs.SetFloat("SFXVolume", 1);
        PlayerPrefs.SetFloat("VoicelinesVolume", 1);
        PlayerPrefs.SetInt("ShowGore", 1);

        CreateEndings();

        foreach (KeyValuePair<string, Ending> ending in endings)
        {
            ending.Value.isUnlocked = false;
            PlayerPrefs.SetInt(ending.Key, 0);
        }

        ResetGameData();
    }

    public static void ResetGameData()
    {
        PlayerPrefs.SetString("Name", "");
        PlayerPrefs.SetString("Pronouns", "she");

        PlayerPrefs.SetString("DialogueID", string.Empty);

        PlayerPrefs.SetInt("HasPlayedGhostHunt", 0);
        PlayerPrefs.SetInt("GhostHuntScore", 0);
        PlayerPrefs.SetInt("HasPlayedAcheron", 0);
        PlayerPrefs.SetInt("HasPlayedMemory", 0);
        PlayerPrefs.SetInt("MemoryScore", 0);

        SetData();
    }
}
