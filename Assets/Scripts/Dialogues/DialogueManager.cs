using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewGraph;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Dialogue startingDialogue;

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
    [SerializeField] private CanvasGroup endingCanvasGroup;
    [SerializeField] private Image endingBadgeImage;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private float endingFadeTime;

    [SerializeField] private string mainMenuSceneName;

    private static Dialogue currentDialogue;

    [SerializeField] private DialogueHolder dialogueHolder;
    private Dictionary<string, Dialogue> dialogues;

    [SerializeField] private ScriptableGraphModel graph;

    [SerializeField] private float timeAfterDialogue;
    private WaitForSeconds timeAfterDialogueWait;
    [SerializeField] private float typingSpeed;
    private WaitForSeconds typingSpeedWait;
    [SerializeField] private float fastTypingSpeed;
    private WaitForSeconds fastTypingSpeedWait;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;
    [SerializeField] private TMP_InputField dialogueInputField;

    private WaitForSeconds wait1Second;

    [SerializeField] private Animator ghostAnimator;
    private static readonly int Move = Animator.StringToHash("Move");

    private Coroutine dialogueCoroutine;
    private Coroutine startDelayCoroutine;

    private bool canClick;
    private bool isTyping;
    private bool isFast;
    private bool isWaiting;

    private Ending currentEnding;

    private string sceneToGoTo;

    private void Awake()
    {
        FadeManager.Show();

#if UNITY_EDITOR
        dialogueHolder.CheckDialogues();
#endif

        if (SaveDataManager.saveData == null)
        {
            Debug.LogError("No game save was loaded");
            return;
        }

        dialogues = new Dictionary<string, Dialogue>();
        foreach (Dialogue dialogue in dialogueHolder.dialogues)
        {
            dialogues.Add(dialogue.name, dialogue);
        }

        startingDialogue = dialogueHolder.startingDialogue;

        string foundName = SaveDataManager.saveData.currentDialogueID;

        if (foundName != string.Empty)
            startingDialogue = FindDialogue(foundName);

        if (answerButtonParent == null || answerButtonPrefab == null || answerCount == 0) return;

        answerButtons = new AnswerButton[answerCount];
        for (int i = 0; i < answerCount; i++)
        {
            AnswerButton button = Instantiate(answerButtonPrefab, answerButtonParent).GetComponent<AnswerButton>();
            answerButtons[i] = button;
            button.Setup(this);
        }

        typingSpeed = SaveDataManager.saveData.textSpeed;
        typingSpeedWait = new WaitForSeconds(typingSpeed);
        fastTypingSpeedWait = new WaitForSeconds(fastTypingSpeed);
        timeAfterDialogueWait = new WaitForSeconds(timeAfterDialogue);
        wait1Second = new WaitForSeconds(1);

        devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
        devInputAction.performed += DevDialogueField;

        textBox.text = string.Empty;
        nameBox.text = string.Empty;
        dialogueBox.SetActive(false);
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
            int score = startingDialogue.minigame switch
            {
                Minigames.GhostHunt when SaveDataManager.saveData.hasPlayedGhostHunt => SaveDataManager.saveData.ghostHuntScore,
                Minigames.Acheron when SaveDataManager.saveData.hasPlayedAcheron => 0,
                Minigames.Memory when SaveDataManager.saveData.hasPlayedMemory => SaveDataManager.saveData.memoryScore,
                _ => -1
            };

            if (score != -1)
            {
                startingDialogue = score >= startingDialogue.scoreToWin
                    ? startingDialogue.wonDialogue
                    : startingDialogue.loseDialogue;
            }
        }

        AudioManager.PlaySound(Sounds.Music);
        FadeManager.StartFade(true, Load);
        devInputAction.Enable();
    }

    private void Load()
    {
        dialogueBox.SetActive(true);
        LoadNewDialogue(startingDialogue);
    }

    public void BoxPress()
    {
        if (!canClick) return;
        switch (isTyping)
        {
            case true when !isFast:
                SkipDialogue();
                break;
            case true when isWaiting:
                SkipWaitDialogue();
                break;
            case false:
                startDelayCoroutine = StartCoroutine(StartDelay());
                break;
        }
    }

    private IEnumerator StartDelay()
    {
        canClick = false;
        AudioManager.PlaySound(Sounds.Dialogue);

        if (currentDialogue.delay > 0)
        {
            bool shouldHide = currentDialogue.answers != null && currentDialogue.answers.Length > 0;

            if (!shouldHide)
                dialogueBox.SetActive(false);

            yield return new WaitForSeconds(currentDialogue.delay);
        }

        switch (currentDialogue.eventToPlay)
        {
            default:
            case Events.None:
                break;
            case Events.GhostZoom:
                ghostAnimator.SetTrigger(Move);
                yield return wait1Second;
                break;
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
            default:
            case Minigames.None:
                break;
            case Minigames.GhostHunt:
                Save();
                StartLoadScene("Ghost Hunt");
                break;
            case Minigames.Acheron:
                Save();
                StartLoadScene("Acheron");
                break;
            case Minigames.Memory:
                Save();
                StartLoadScene("Memory");
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

        if (currentDialogue.goreSprite != null && SaveDataManager.saveData.showGore)
            charImage.sprite = currentDialogue.goreSprite;
        else if (currentDialogue.sprite != null)
            charImage.sprite = currentDialogue.sprite;

        if (currentDialogue.goreBackground != null && SaveDataManager.saveData.showGore)
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
        canClick = true;
        nextButton.interactable = true;
        string fullText = currentDialogue.text;

        fullText = fullText.Replace("{name}", SaveDataManager.saveData.name);
        fullText = fullText.Replace("{pronoun}", SaveDataManager.saveData.pronouns);

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
            StartLoadScene(mainMenuSceneName);
            return;
        }

        ShowEnding();
    }

    private void ShowEnding()
    {
        endingBadgeImage.sprite = currentEnding.badgeSprite;
        endingText.text = currentEnding.endingName;
        StartCoroutine(EndingFade());
    }

    private IEnumerator EndingFade()
    {
        endingCanvasGroup.alpha = 0;
        endingCanvas.gameObject.SetActive(true);

        for (float i = 0; i <= endingFadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > endingFadeTime) i = endingFadeTime;

            float fillAmount = i / endingFadeTime;

            endingCanvasGroup.alpha = Mathf.Lerp(0, 1, fillAmount);

            yield return null;
        }

        endingCanvasGroup.alpha = 1;
    }

    public void MainMenuButton()
    {
        SaveDataManager.saveData.textSpeed = typingSpeed;
        SaveDataManager.saveData.currentDialogueID = currentDialogue.name;
        SaveDataManager.Save();
        StartLoadScene(mainMenuSceneName);
    }

    public void LeaveAfterEnding()
    {
        SaveDataManager.saveData.endings[currentEnding.endingID].isUnlocked = true;
        SaveDataManager.Save();
        SaveDataManager.ResetGameData();
        StartLoadScene(mainMenuSceneName);
    }

    private static void Save()
    {
        SaveDataManager.saveData.currentDialogueID = currentDialogue.name;
        SaveDataManager.Save();
    }

    private void DevDialogueField(InputAction.CallbackContext context)
    {
        if (dialogueInputField.gameObject.activeSelf)
            dialogueInputField.gameObject.SetActive(false);
        else
        {
            dialogueInputField.text = string.Empty;
            dialogueInputField.gameObject.SetActive(true);
        }
    }

    public void DevDialogueFieldEnter()
    {
        string dialogueToFind = dialogueInputField.text;

        Dialogue dialogue = FindDialogue(dialogueToFind);

        if (dialogue == null) return;

        if (startDelayCoroutine != null) StopCoroutine(startDelayCoroutine);
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

        dialogueInputField.gameObject.SetActive(false);
        LoadNewDialogue(dialogue);
    }

    private void StartLoadScene(string sceneName)
    {
        devInputAction.performed -= DevDialogueField;
        devInputAction.Disable();
        sceneToGoTo = sceneName;
        FadeManager.StartFade(false, LoadScene);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToGoTo);
    }

    private Dialogue FindDialogue(string id)
    {
        return dialogues[id];
    }
}
