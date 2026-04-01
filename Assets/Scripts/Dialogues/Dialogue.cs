using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string charName;
    public string text;
    public AudioClip voiceline;
    public string nextDialogueID;
    public float delay;
    public string[] answersID;
    public Events eventToPlay;
    public Sounds soundToPlay;
    public bool loopSound;
    public Sounds musicToPlay;
    public Minigames minigame;
    public string wonDialogueID;
    public string loseDialogueID;
    public Ending ending;
    public Sprite sprite;
    public Sprite background;
}
