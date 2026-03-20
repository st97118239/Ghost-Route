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
    public Minigames minigame;
    public int scoreToWin;
    public string wonDialogueID;
    public string loseDialogueID;
    public Ending ending;
    public Sprite sprite;
    public Sprite goreSprite;
    public Sprite background;
    public Sprite goreBackground;
}
