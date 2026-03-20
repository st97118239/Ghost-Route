using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Answer")]
public class Answer : ScriptableObject
{
    public string text;
    public Dialogue dialogue;
    public string dialogueID;
}