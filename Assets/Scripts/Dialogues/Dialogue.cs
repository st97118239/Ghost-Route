using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public string charName;
    public string text;
    public AudioClip voiceline;
    public Dialogue nextDialogue;
    public float delay;
    public Answer[] answers;
    public Events eventToPlay;
    public Minigames minigame;
    public int scoreToWin;
    public Dialogue wonDialogue;
    public Dialogue loseDialogue;
    public Ending ending;
    public Sprite sprite;
    public Sprite goreSprite;
    public Sprite background;
    public Sprite goreBackground;
}
