using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private DialogueManager dialogueManager;

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;

    private Answer answer;

    public void Setup(DialogueManager givenManager)
    {
        dialogueManager = givenManager;
        gameObject.SetActive(false);
    }

    public void Load(Answer givenAnswer)
    {
        answer = givenAnswer;
        text.text = answer.text;
        gameObject.SetActive(true);
    }

    public void Press()
    {
        dialogueManager.AnswerPressed(answer);
    }
}
