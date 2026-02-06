using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        if (endings == null)
        {
            CreateEndings();
        }

        foreach ((string id, Ending value) in endings)
        {
            value.isUnlocked = PlayerPrefs.GetInt(id) != 0;
        }

        currentDialogueID = PlayerPrefs.GetString("DialogueID");
    }

    private static void CreateEndings()
    {
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

        foreach (KeyValuePair<string, Ending> ending in endings) 
            PlayerPrefs.SetInt(ending.Key, ending.Value.isUnlocked ? 1 : 0);

        PlayerPrefs.SetString("DialogueID", currentDialogueID);
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

        PlayerPrefs.SetString("Name", "");
        PlayerPrefs.SetString("Pronouns", "she");

        CreateEndings();

        foreach (KeyValuePair<string, Ending> ending in endings)
        {
            ending.Value.isUnlocked = false;
            PlayerPrefs.SetInt(ending.Key, 0);
        }

        PlayerPrefs.SetString("DialogueID", string.Empty);

        SetData();
    }
}
