using System.Collections;
#if UNITY_EDITOR
using System.IO;
using System.Linq; // LINQ ONLY WORKS IN EDITOR
#endif
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
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

    [SerializeField] private Canvas endingCanvas;
    [SerializeField] private Image endingBadgeImage;
    [SerializeField] private TMP_Text endingText;

    [SerializeField] private string mainMenuSceneName;

    private static Dialogue currentDialogue;

    [SerializeField] private Dialogue[] dialogues;

    [SerializeField] private float timeAfterDialogue;
    private WaitForSeconds timeAfterDialogueWait;
    [SerializeField] private float typingSpeed;
    private WaitForSeconds typingSpeedWait;
    [SerializeField] private float fastTypingSpeed;
    private WaitForSeconds fastTypingSpeedWait;

    private Coroutine dialogueCoroutine;

    private bool isTyping;
    private bool isFast;
    private bool isWaiting;

    private Ending currentEnding;

    private void Awake()
    {
#if UNITY_EDITOR
        string[] files = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Dialogues");
        int amt = files.Count(file => !file.EndsWith(".meta"));

        if (dialogues.Length < amt)
            Debug.LogWarning("Not all dialogues are in the dialogues variable. Please put them all in.");
        else if (dialogues.Length > amt)
            Debug.LogWarning("There are too many dialogues in the dialogues variable.");
        else if (dialogues.Length == amt)
            Debug.Log("All dialogues are in the variable.");
#endif

        string foundName = SaveData.currentDialogueID;

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

        typingSpeed = SaveData.textSpeed;
        typingSpeedWait = new WaitForSeconds(typingSpeed);
        fastTypingSpeedWait = new WaitForSeconds(fastTypingSpeed);
        timeAfterDialogueWait = new WaitForSeconds(timeAfterDialogue);
    }

    private void Start()
    {
        if (startingDialogue == null)
        {
            Debug.LogError("No Starting Dialogue is set");
            return;
        }

        if (startingDialogue.minigame != Minigames.None)
        {
            int score = -1;
            switch (startingDialogue.minigame)
            {
                case Minigames.GhostHunt when SaveData.hasPlayedGhostHunt:
                    score = SaveData.ghostHuntScore;
                    break;
                case Minigames.Acheron when SaveData.hasPlayedAcheron:
                    score = SaveData.acheronScore;
                    break;
                case Minigames.Memory when SaveData.hasPlayedMemory:
                    score = SaveData.memoryScore;
                    break;
            }

            if (score != -1)
            {
                startingDialogue = score >= startingDialogue.scoreToWin
                    ? startingDialogue.wonDialogue
                    : startingDialogue.loseDialogue;
            }
        }
        
        LoadNewDialogue(startingDialogue);
    }

    public void BoxPress()
    {
        switch (isTyping)
        {
            case true when !isFast:
                SkipDialogue();
                break;
            case true when isWaiting:
                SkipWaitDialogue();
                break;
            case false:
                StartCoroutine(StartDelay());
                break;
        }
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
        }

        DelayFinished();
    }

    private void DelayFinished()
    {
        if (currentDialogue.ending != null)
        {
            GetEnding();
            return;
        }

        switch (currentDialogue.minigame)
        {
            case Minigames.None:
                break;
            case Minigames.GhostHunt:
                Save();
                SceneManager.LoadScene("Ghost Hunt");
                break;
            case Minigames.Acheron:
                Save();
                SceneManager.LoadScene("Acheron");
                break;
            case Minigames.Memory:
                Save();
                SceneManager.LoadScene("Memory");
                return;
        }

        dialogueBox.SetActive(true);

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
        nameBox.text = currentDialogue.charName;
        dialogueBox.SetActive(true);

        if (currentDialogue.goreSprite != null && SaveData.showGore)
            charImage.sprite = currentDialogue.goreSprite;
        else if (currentDialogue.sprite != null)
            charImage.sprite = currentDialogue.sprite;

        if (currentDialogue.goreBackground != null && SaveData.showGore)
            backgroundImage.sprite = currentDialogue.goreBackground;
        else if (currentDialogue.background != null)
            backgroundImage.sprite = currentDialogue.background;

        if (currentDialogue.voiceline != null)
            AudioManager.PlayVoiceline(currentDialogue.voiceline);

        dialogueCoroutine = StartCoroutine(ShowDialogue());
    }

    private IEnumerator ShowDialogue()
    {
        isTyping = true;
        nextButton.interactable = true;
        string fullText = currentDialogue.text;

        fullText = fullText.Replace("{name}", SaveData.name);
        fullText = fullText.Replace("{pronoun}", SaveData.pronouns);

        for (int i = 0; i < fullText.Length + 1; i++)
        {
            textBox.text = fullText[..i];
            if (isFast)
                yield return fastTypingSpeedWait;
            else
                yield return typingSpeedWait;
        }

        if (isFast)
        {
            isWaiting = true;
            yield return timeAfterDialogueWait;
            isFast = false;
            isWaiting = false;
        }
        isTyping = false;
        dialogueCoroutine = null;
    }

    private void SkipDialogue()
    {
        isFast = true;
    }
    
    private void SkipWaitDialogue()
    {
        StopCoroutine(dialogueCoroutine);
        isWaiting = false;
        isFast = false;
        isTyping = false;
        dialogueCoroutine = null;
    }

    private void LoadAnswers()
    {
        nextButton.interactable = false;

        answerButtonParent.gameObject.SetActive(true);

        int max = currentDialogue.answers.Length;

        if (max > answerButtons.Length) max = answerButtons.Length;

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

    private void GetEnding()
    {
        currentEnding = currentDialogue.ending;
        bool hasFound = MainMenuManager.endings.Any(t => t.endingID == currentEnding.endingID);

        if (!hasFound)
        {
            Debug.LogError("Ending ID wasn't found in MainMenuManager endings");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        ShowEnding();
    }

    private void ShowEnding()
    {
        endingBadgeImage.sprite = currentEnding.badgeSprite;
        endingText.text = currentEnding.endingName;
        endingCanvas.gameObject.SetActive(true);
    }

    public void MainMenuButton()
    {
        SaveData.textSpeed = typingSpeed;
        SaveData.currentDialogueID = currentDialogue.name;
        SaveData.Save();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LeaveAfterEnding()
    {
        SaveData.endings[currentEnding.endingID].isUnlocked = true;
        SaveData.Save();
        SaveData.ResetGameData();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private static void Save()
    {
        SaveData.currentDialogueID = currentDialogue.name;
        SaveData.Save();
    }
}
