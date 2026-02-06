using UnityEngine;

[CreateAssetMenu(menuName = "Ending")]
public class Ending : ScriptableObject
{
    public string endingName;
    public string description;
    public Sprite badgeSprite;
    public string endingID;
    public bool isUnlocked;
}