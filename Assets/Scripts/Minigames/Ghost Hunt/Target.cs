using UnityEngine;

[CreateAssetMenu(menuName = "Ghost Hunt Target")]
public class Target : ScriptableObject
{
    public Sprite sprite;
    public TargetType type;
    public int points;
}