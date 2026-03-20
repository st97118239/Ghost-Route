using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string charName;
    public string text;
    public AudioClip voiceline;
    public Dialogue nextDialogue;
    public string nextDialogueID;
    public float delay;
    public Answer[] answers;
    public string[] answersID;
    public Events eventToPlay;
    public Minigames minigame;
    public int scoreToWin;
    public Dialogue wonDialogue;
    public string wonDialogueID;
    public Dialogue loseDialogue;
    public string loseDialogueID;
    public Ending ending;
    public Sprite sprite;
    public Sprite goreSprite;
    public Sprite background;
    public Sprite goreBackground;
}
