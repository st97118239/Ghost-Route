using System;
using System.Collections.Generic;
using UnityEngine;

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

    public bool hasFinishedGhostHunt;
    public bool hasStartedGhostHunt;

    public bool hasFinishedAcheron;
    public bool hasStartedAcheron;
    public bool hasSeenAcheronDialogue;

    public bool hasFinishedMemory;
    public bool hasStartedMemory;
    public bool hasWonMemory;

    public void Reset(Ending[] givenEndings)
    {
        textSpeed = 0.04f;
        windowType = (int)Screen.fullScreenMode;
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

        hasFinishedGhostHunt = false;
        hasStartedGhostHunt = false;

        hasFinishedAcheron = false;
        hasStartedAcheron = false;
        hasSeenAcheronDialogue = false;

        hasFinishedMemory = false;
        hasFinishedMemory = false;
        hasWonMemory = false;
    }
}
