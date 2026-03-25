using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public float textSpeed;
    public int windowType;
    public float bgmVolume;
    public float sfxVolume;
    public float voicelinesVolume;

    public string name;
    public string pronouns;

    public Dictionary<string, Ending> endings;
    public bool[] endingUnlocked;

    public string currentDialogueID;

    public bool hasPlayedGhostHunt;
    public int ghostHuntScore;

    public bool hasPlayedAcheron;

    public bool hasPlayedMemory;
    public int memoryScore;

    public void Reset(Ending[] givenEndings)
    {
        textSpeed = 0.04f;
        windowType = 1;
        bgmVolume = 1;
        sfxVolume = 1;
        voicelinesVolume = 1;

        endings = new();
        foreach (Ending ending in givenEndings)
        {
            endings.Add(ending.endingID, ending);
            ending.isUnlocked = false;
        }

        endingUnlocked = new bool[endings.Count];

        for (int i = 0; i < endingUnlocked.Length; i++) 
            endingUnlocked[i] = false;

        ResetGameSave();
    }

    public void ResetGameSave()
    {
        name = string.Empty;
        pronouns = string.Empty;

        currentDialogueID = string.Empty;

        hasPlayedGhostHunt = false;
        ghostHuntScore = -1;

        hasPlayedAcheron = false;

        hasPlayedMemory = false;
        memoryScore = -1;
    }
}
