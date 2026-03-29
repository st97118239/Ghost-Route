using UnityEngine;

[CreateAssetMenu(menuName = "Ghost Hunt/Targets Holder")]
public class TargetsHolder : ScriptableObject
{
    public Target[] ghosts;
    public Target[] bunnies;
    public Target[] deer;
}