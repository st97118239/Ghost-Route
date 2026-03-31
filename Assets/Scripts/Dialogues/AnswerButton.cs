using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private DialogueManager dialogueManager;

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    [SerializeField] private float spawnOffset = 1311.065f;
    [SerializeField] private float moveSpeed = 10;

    private Answer answer;

    private int idx;

    public void Setup(DialogueManager givenManager, int givenIdx)
    {
        dialogueManager = givenManager;
        gameObject.SetActive(false);
        idx = givenIdx;
    }

    public void Load(Answer givenAnswer)
    {
        answer = givenAnswer;

        string fullText = answer.text;
        fullText = fullText.Replace("{name}", SaveDataManager.saveData.name);
        fullText = fullText.Replace("{pronoun}", SaveDataManager.saveData.pronouns);

        text.text = fullText;
        rectTrans.anchoredPosition = new Vector2(spawnOffset, rectTrans.anchoredPosition.y);
        gameObject.SetActive(true);
        StartCoroutine(ButtonAnimation());
    }

    private IEnumerator ButtonAnimation()
    {
        yield return null;
        layoutGroup.enabled = true;
        layoutGroup.enabled = false;
        layoutGroup.enabled = true;
        yield return null;

        layoutGroup.enabled = false;
        yield return null;

        Vector2 targetPos = new(rectTrans.anchoredPosition.x - spawnOffset, rectTrans.anchoredPosition.y);
        yield return null;

        while (rectTrans.anchoredPosition != targetPos)
        {
            rectTrans.anchoredPosition = Vector2.MoveTowards(rectTrans.anchoredPosition, targetPos, moveSpeed);

            yield return Time.fixedDeltaTime;
        }

        if (idx == 0)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Press()
    {
        dialogueManager.AnswerPressed(answer);
    }
}
