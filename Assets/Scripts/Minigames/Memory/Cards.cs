using UnityEngine;

[CreateAssetMenu(menuName = "Memory/Card")]
public class Cards : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public AudioClip playerScoreVoiceline;
    public AudioClip opponentScoreVoiceline;
}