using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // TODO:
    // Answer options
    // Background switcher
    // Typewriter effect
    // Minigame starter

    [SerializeField] private Dialogue startingDialogue;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private TMP_Text nameBox;
    [SerializeField] private Image charImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private int answerCount;
    [SerializeField] private Transform answerButtonParent;
    [SerializeField] private GameObject answerButtonPrefab;
    private AnswerButton[] answerButtons;

    [SerializeField] private string mainMenuSceneName;

    private static Dialogue currentDialogue;

    [SerializeField] private Dialogue[] dialogues;

    private void Awake()
    {
        string foundName = PlayerPrefs.GetString("DialogueID");

        if (foundName != string.Empty)
        {
            foreach (Dialogue dialogue in dialogues)
            {
                if (dialogue.name != foundName) continue;
                startingDialogue = dialogue;
                break;
            }
        }

        if (answerButtonParent == null || answerButtonPrefab == null || answerCount == 0) return;

        answerButtons = new AnswerButton[answerCount];
        for (int i = 0; i < answerCount; i++)
        {
            AnswerButton button = Instantiate(answerButtonPrefab, answerButtonParent).GetComponent<AnswerButton>();
            answerButtons[i] = button;
            button.Setup(this);
        }
    }

    private void Start()
    {
        if (startingDialogue == null)
        {
            Debug.LogError("No Starting Dialogue is set");
            return;
        }
        LoadNewDialogue(startingDialogue);
    }

    public void BoxPress()
    {
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        if (currentDialogue.delay > 0)
        {
            bool shouldHide = currentDialogue.answers != null && currentDialogue.answers.Length > 0;

            if (!shouldHide)
            {
                dialogueBox.SetActive(false);
            }

            yield return new WaitForSeconds(currentDialogue.delay);

            if (!shouldHide)
            {
                dialogueBox.SetActive(true);
            }
        }

        DelayFinished();
    }

    private void DelayFinished()
    {
        if (currentDialogue.answers != null && currentDialogue.answers.Length > 0)
        {
            LoadAnswers();
            return;
        }

        if (currentDialogue.nextDialogue == null) return;

        LoadNewDialogue(currentDialogue.nextDialogue);
    }

    private void LoadNewDialogue(Dialogue givenDialogue)
    {
        currentDialogue = givenDialogue;
        textBox.text = currentDialogue.text;
        nameBox.text = currentDialogue.charName;
        dialogueBox.SetActive(true);
        if (currentDialogue.sprite != null)
            charImage.sprite = currentDialogue.sprite;
        if (currentDialogue.background != null)
            backgroundImage.sprite = currentDialogue.background;
        nextButton.interactable = true;
    }

    private void LoadAnswers()
    {
        nextButton.interactable = false;

        answerButtonParent.gameObject.SetActive(true);

        int max = answerButtons.Length;

        if (max > 3) max = 3;

        for (int i = 0; i < max; i++)
        {
            AnswerButton button = answerButtons[i];
            button.Load(currentDialogue.answers[i]);
        }
    }

    public void AnswerPressed(Answer answer)
    {
        foreach (AnswerButton t in answerButtons)
        {
            t.gameObject.SetActive(false);
        }
        answerButtonParent.gameObject.SetActive(false);

        LoadNewDialogue(answer.dialogue);
    }

    public void MainMenuButton()
    {
        PlayerPrefs.SetString("DialogueID", currentDialogue.name);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
