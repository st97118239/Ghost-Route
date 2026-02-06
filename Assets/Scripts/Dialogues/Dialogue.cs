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
    public Minigames minigame;
    public Ending ending;
    public Sprite sprite;
    public Sprite background;
}
