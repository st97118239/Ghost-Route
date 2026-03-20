#if UNITY_EDITOR
using System.IO;
using System.Linq;
#endif
using NewGraph;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Holder")]
public class DialogueHolder : ScriptableObject
{
    public Dialogue startingDialogue;
    public Dialogue[] dialogues;
    public Answer[] answers;

    private ScriptableGraphModel graph;

#if UNITY_EDITOR
    public void CheckDialogues()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Dialogues");
        int amt = files.Count(file => !file.EndsWith(".meta"));

        if (dialogues.Length < amt)
            Debug.LogWarning("Not all dialogues are in the dialogues variable. Please put them all in.");
        else if (dialogues.Length > amt)
            Debug.LogWarning("There are too many dialogues in the dialogues variable.");
        else if (dialogues.Length == amt)
            Debug.Log("All dialogues are in the variable.");
    }
#endif
}
