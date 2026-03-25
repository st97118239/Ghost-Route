using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private string startingDialogue;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private TMP_Text nameBox;
    [SerializeField] private Image charImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Animator backgroundAnimator;
    [SerializeField] private float charDisappearTime;

    [SerializeField] private AnswerButton[] answerButtons;

    [SerializeField] private AcheronManager acheronManager;

    [SerializeField] private Canvas endingCanvas;
    [SerializeField] private CanvasGroup endingCanvasGroup;
    [SerializeField] private Image endingBadgeImage;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private float endingFadeTime;

    [SerializeField] private string mainMenuSceneName;

    [SerializeField] private bool autoContinue;
    [SerializeField] private bool shouldUseSave;

    private Dialogue currentDialogue;

    [SerializeField] private DialogueHolder dialogueHolder;
    private Dictionary<string, Dialogue> dialogues;
    private Dictionary<string, Answer> answers;

    [SerializeField] private float timeAfterDialogue;
    private WaitForSeconds timeAfterDialogueWait;
    [SerializeField] private float typingSpeed;
    private WaitForSeconds typingSpeedWait;
    [SerializeField] private float fastTypingSpeed;
    private WaitForSeconds fastTypingSpeedWait;

    [SerializeField] private bool enableDevInput;
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;
    [SerializeField] private TMP_InputField dialogueInputField;

    private WaitForSeconds wait1Second;

    [SerializeField] private Animator ghostAnimator;
    private static readonly int Move = Animator.StringToHash("Move");

    private Coroutine dialogueCoroutine;
    private Coroutine startDelayCoroutine;

    private Sounds playingSound;
    private bool isPlayingSound;

    private bool canClick;
    private bool isTyping;
    private bool isFast;
    private bool isWaiting;

    private Ending currentEnding;

    private string sceneToGoTo;

    private void Awake()
    {
        FadeManager.Show();

        if (SaveDataManager.saveData == null)
        {
            Debug.LogError("No game save was loaded");
            return;
        }

        dialogues = new Dictionary<string, Dialogue>();
        foreach (Dialogue dialogue in dialogueHolder.dialogues) 
            dialogues.Add(dialogue.name, dialogue);

        answers = new Dictionary<string, Answer>();
        foreach (Answer answer in dialogueHolder.answers) 
            answers.Add(answer.name, answer);

        startingDialogue = dialogueHolder.startingDialogueID;

        if (shouldUseSave)
        {
            string foundName = SaveDataManager.saveData.currentDialogueID;

            if (foundName != string.Empty)
                startingDialogue = foundName;
        }

        foreach (AnswerButton button in answerButtons) 
            button.Setup(this);

        typingSpeed = SaveDataManager.saveData.textSpeed;
        typingSpeedWait = new WaitForSeconds(typingSpeed);
        fastTypingSpeedWait = new WaitForSeconds(fastTypingSpeed);
        timeAfterDialogueWait = new WaitForSeconds(timeAfterDialogue);
        wait1Second = new WaitForSeconds(1);

        if (enableDevInput)
        {
            devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
            devInputAction.performed += DevDialogueField;
        }

        if (textBox != null)
            textBox.text = string.Empty;
        if (nameBox != null)
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

        Dialogue dialogue = FindDialogue(startingDialogue);

        if (dialogue.minigame != Minigames.None)
        {
            int score = dialogue.minigame switch
            {
                Minigames.GhostHunt when SaveDataManager.saveData.hasPlayedGhostHunt => SaveDataManager.saveData.ghostHuntScore,
                Minigames.Acheron when SaveDataManager.saveData.hasPlayedAcheron => 0,
                Minigames.Memory when SaveDataManager.saveData.hasPlayedMemory => SaveDataManager.saveData.memoryScore,
                _ => -1
            };

            if (score != -1)
            {
                startingDialogue = score >= dialogue.scoreToWin
                    ? dialogue.wonDialogueID
                    : dialogue.loseDialogueID;

                if (startingDialogue == string.Empty)
                    startingDialogue = dialogue.nextDialogueID;
            }
        }

        if (backgroundImage != null && dialogue.background != null)
            backgroundImage.sprite = dialogue.background;
        if (charImage != null)
        {
            charImage.sprite = dialogue.sprite;
            charImage.color = dialogue.sprite == null ? Color.clear : Color.white;
        }

        AudioManager.PlaySound(Sounds.Music, false);
        FadeManager.StartFade(true, Load, Color.black);
        if (enableDevInput)
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
        AudioManager.PlaySound(Sounds.Dialogue, false);

        if (currentDialogue.delay > 0)
        {
            bool shouldHide = (currentDialogue.answersID != null && currentDialogue.answersID.Length > 0) || autoContinue;

            if (!shouldHide)
                dialogueBox.SetActive(false);

            yield return new WaitForSeconds(currentDialogue.delay);
        }

        playingSound = currentDialogue.soundToPlay;
        if (currentDialogue.soundToPlay != Sounds.None && !isPlayingSound) 
            StartCoroutine(PlaySound());

        switch (currentDialogue.eventToPlay)
        {
            default:
            case Events.None:
                break;
            case Events.GhostZoom:
                ghostAnimator.SetTrigger(Move);
                yield return wait1Second;
                break;
            case Events.FadeBlack:
                FadeManager.StartFade(false, FadeBetweenDialoguesBlack, Color.black);
                yield break;
            case Events.FadeWhite:
                FadeManager.StartFade(false, FadeBetweenDialoguesWhite, Color.white);
                yield break;
            case Events.CameraLookAround:
                StartCoroutine(EventLookAround());
                yield break;
            case Events.ZoomArcadeMachine:
                backgroundAnimator.SetBool("ZoomArcade", !backgroundAnimator.GetBool("ZoomArcade"));
                yield return wait1Second;
                break;
            case Events.ZoomDoor:
                backgroundAnimator.SetBool("ZoomDoor", !backgroundAnimator.GetBool("ZoomDoor"));
                yield return wait1Second;
                break;
            case Events.ZoomTable:
                backgroundAnimator.SetBool("ZoomTable", !backgroundAnimator.GetBool("ZoomTable"));
                yield return wait1Second;
                break;
            case Events.StartAcheron:
                StartAcheron();
                yield break;
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
                return;
            case Minigames.Acheron:
                Save();
                StartLoadScene("Acheron");
                return;
            case Minigames.Memory:
                Save();
                StartLoadScene("Memory");
                return;
        }

        //if (textBox.text != string.Empty && textBox.text != "")
        dialogueBox.SetActive(true);

        if (currentDialogue.answersID != null && currentDialogue.answersID.Length > 0)
        {
            LoadAnswers();
            return;
        }

        if (string.IsNullOrEmpty(currentDialogue.nextDialogueID)) return;

        LoadNewDialogue(currentDialogue.nextDialogueID);
    }

    private void LoadNewDialogue(string dialogueID)
    {
        currentDialogue = FindDialogue(dialogueID);

        dialogueCoroutine = StartCoroutine(ShowDialogue());
    }

    private IEnumerator ShowDialogue()
    {
        if (nameBox != null)
            nameBox.text = currentDialogue.charName;
        if (currentDialogue.text == string.Empty)
        {
            textBox.text = string.Empty;
            dialogueBox.SetActive(false);
        }
        else
            dialogueBox.SetActive(true);

        if (charImage != null)
        {
            charImage.sprite = currentDialogue.sprite;
            charImage.color = currentDialogue.sprite == null ? Color.clear : Color.white;
        }
        if (backgroundImage != null && currentDialogue.background != null) 
            backgroundImage.sprite = currentDialogue.background;
        float voiceLineLength = 0f;
        if (currentDialogue.voiceline != null)
            voiceLineLength = AudioManager.PlayVoiceline(currentDialogue.voiceline);

        isTyping = true;
        canClick = true;
        if (nextButton != null)
            nextButton.interactable = true;
        string fullText = currentDialogue.text;

        fullText = fullText.Replace("{name}", SaveDataManager.saveData.name);
        fullText = fullText.Replace("{pronoun}", SaveDataManager.saveData.pronouns);

        int forLoopMax = fullText.Length + 1;
        for (int i = 1; i < forLoopMax; i++)
        {
            string text = fullText[..i];
            if (i < fullText.Length)
            {
                if (fullText[i - 1].ToString() == "<")
                {
                    int charsTillEnd = 0;
                    while (true)
                    {
                        charsTillEnd++;
                        if (fullText[i - 1 + charsTillEnd].ToString() == ">")
                            break;
                    }

                    i += charsTillEnd;
                    text = fullText[..i];
                }
            }
            textBox.text = text;
            if (isFast)
            {
                yield return fastTypingSpeedWait;
                voiceLineLength -= fastTypingSpeed;
            }
            else
            {
                yield return typingSpeedWait;
                voiceLineLength -= typingSpeed;
            }
        }

        if (isFast)
        {
            isWaiting = true;
            yield return timeAfterDialogueWait;
            voiceLineLength -= timeAfterDialogue;
            isFast = false;
            isWaiting = false;
        }

        isTyping = false;
        dialogueCoroutine = null;

        if (!autoContinue && textBox.text != string.Empty && textBox.text != "") yield break;

        if (!autoContinue)
        {
            yield return wait1Second;
            if (voiceLineLength > 0)
                yield return new WaitForSeconds(voiceLineLength);
        }

        startDelayCoroutine = StartCoroutine(StartDelay());
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

        int max = currentDialogue.answersID.Length;

        if (max > answerButtons.Length) max = answerButtons.Length;

        for (int i = 0; i < max; i++)
        {
            AnswerButton button = answerButtons[i];
            Answer answer = FindAnswer(currentDialogue.answersID[i]);
            button.Load(answer);
        }
    }

    public void AnswerPressed(Answer answer)
    {
        foreach (AnswerButton t in answerButtons) 
            t.gameObject.SetActive(false);

        LoadNewDialogue(answer.dialogueID);
    }

    private void FadeBetweenDialoguesBlack()
    {
        FadeManager.StartFade(true, DelayFinished, Color.black);
    }

    private void FadeBetweenDialoguesWhite()
    {
        StartCoroutine(WaitBetweenWhiteFade());
    }

    private IEnumerator WaitBetweenWhiteFade()
    {
        yield return wait1Second;

        FadeManager.StartFade(true, DelayFinished, Color.white);
    }

    private IEnumerator EventLookAround()
    {
        dialogueBox.SetActive(false);

        yield return null;

        for (float i = 0; i < charDisappearTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > charDisappearTime) i = charDisappearTime;

            float alphaAmount = i / charDisappearTime;
            charImage.color = new Color(255, 255, 255, Mathf.Lerp(1, 0, alphaAmount));

            yield return null;
        }

        charImage.color = Color.clear;

        backgroundAnimator.SetTrigger("LookAround");
    }

    public IEnumerator EventLookAroundFinished()
    {
        yield return null;

        for (float i = 0; i < charDisappearTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > charDisappearTime) i = charDisappearTime;

            float alphaAmount = i / charDisappearTime;
            charImage.color = new Color(255, 255, 255, Mathf.Lerp(0, 1, alphaAmount));

            yield return null;
        }

        charImage.color = Color.white;

        DelayFinished();
    }

    private IEnumerator PlaySound()
    {
        isPlayingSound = true;

        WaitForSeconds wait = null;
        while (playingSound != Sounds.None)
        {
            if (wait == null)
            {
                float delay = AudioManager.PlaySound(playingSound, true);
                wait = new WaitForSeconds(delay);
            }
            else
                AudioManager.PlaySound(playingSound, true);

            yield return wait;
        }

        isPlayingSound = false;
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
        if (shouldUseSave) 
            Save();
        StartLoadScene(mainMenuSceneName);
    }

    public void LeaveAfterEnding()
    {
        if (shouldUseSave)
        {
            SaveDataManager.saveData.endings[currentEnding.endingID].isUnlocked = true;
            SaveDataManager.Save();
            SaveDataManager.ResetGameData();
        }
        StartLoadScene(mainMenuSceneName);
    }

    private void Save()
    {
        if (!shouldUseSave) return;
        SaveDataManager.saveData.currentDialogueID = currentDialogue.name;
        SaveDataManager.Save();
    }

    private void StartAcheron()
    {
        if (acheronManager == null) return;

        dialogueBox.SetActive(false);

        acheronManager.UnlockButton();

        gameObject.SetActive(false);
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
        string dialogueID = dialogueInputField.text;

        Dialogue dialogue = FindDialogue(dialogueID);

        if (dialogue == null) return;

        if (startDelayCoroutine != null) StopCoroutine(startDelayCoroutine);
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

        foreach (AnswerButton t in answerButtons)
            t.gameObject.SetActive(false);

        dialogueInputField.gameObject.SetActive(false);
        LoadNewDialogue(dialogueID);
    }

    private void StartLoadScene(string sceneName)
    {
        if (enableDevInput)
        {
            devInputAction.performed -= DevDialogueField;
            devInputAction.Disable();
        }
        sceneToGoTo = sceneName;
        FadeManager.StartFade(false, LoadScene, Color.black);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToGoTo);
    }

    private Dialogue FindDialogue(string id)
    {
        if (!string.IsNullOrEmpty(id)) return dialogues[id];
        Debug.LogWarning("Empty id given");
        return null;
    }

    private Answer FindAnswer(string id)
    {
        if (!string.IsNullOrEmpty(id)) return answers[id];
        Debug.LogWarning("Empty id given");
        return null;
    }
}
