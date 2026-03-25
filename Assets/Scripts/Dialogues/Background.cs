using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;

    private void EventLookAroundFinished()
    {
        StartCoroutine(dialogueManager.EventLookAroundFinished());
    }
}