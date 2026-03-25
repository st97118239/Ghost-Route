using NewGraph;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Holder")]
public class DialogueHolder : ScriptableObject
{
    public ScriptableGraphModel graph;
    public string startingDialogueID;
    public Dialogue[] dialogues;
    public Answer[] answers;
}
