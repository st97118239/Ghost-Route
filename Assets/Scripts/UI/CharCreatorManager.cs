using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharCreatorManager : MonoBehaviour
{
    [SerializeField] private MainMenuManager mainMenu;
    [SerializeField] private Canvas charCreatorCanvas;

    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_InputField nameInput;

    [SerializeField] private GameObject pronounsPanel;
    [SerializeField] private int pronounsIdx;
    [SerializeField] private Image[] pronounsCheckmarks;
    [SerializeField] private Pronoun[] pronouns;

    [SerializeField] private Button confirmButton;
    [SerializeField] private string gameSceneName;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;
    [SerializeField] private TMP_InputField dialogueInputField;
    [SerializeField] private DialogueHolder dialogueHolder;
    private bool invalidDialogueID;

    [SerializeField] private MainMenuBackground backgroundChanger;

    private void Awake()
    {
        pronounsIdx = 1;
        devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
    }

    public void Show()
    {
        charCreatorCanvas.gameObject.SetActive(true);
        backgroundChanger.ChangeBackground(false);

        for (int i = 0; i < pronounsCheckmarks.Length; i++)
            pronounsCheckmarks[i].gameObject.SetActive(i == pronounsIdx);

        confirmButton.interactable = nameInput.text != string.Empty;

        dialogueInputField.gameObject.SetActive(false);
        devInputAction.performed += DevShowInputField;
        devInputAction.Enable();
    }

    public void CloseAllButtons(int idxToKeep)
    {
        if (idxToKeep != 0 && namePanel.activeSelf)
            NameButton();
        if (idxToKeep != 1 && pronounsPanel.activeSelf)
            PronounsButton();
    }

    public void NameButton()
    {
        namePanel.SetActive(!namePanel.activeSelf);
    }

    public void SelectNameField()
    {
        if (dialogueInputField.gameObject.activeSelf) return;

        devInputAction.Disable();
        devInputAction.performed -= DevShowInputField;
    }

    public void DeselectNameField()
    {
        if (dialogueInputField.gameObject.activeSelf) return;

        devInputAction.performed += DevShowInputField;
        devInputAction.Enable();
    }

    public void UpdateName()
    {
        if (nameInput.text == string.Empty || nameInput.text.StartsWith(" ") || invalidDialogueID)
            confirmButton.interactable = false;
        else
            confirmButton.interactable = true;
    }

    public void PronounsButton()
    {
        pronounsPanel.SetActive(!pronounsPanel.activeSelf);
    }

    public void PronounsButton(int idx)
    {
        pronounsIdx = idx;

        for (int i = 0; i < pronounsCheckmarks.Length; i++)
            pronounsCheckmarks[i].gameObject.SetActive(i == idx);

        PronounsButton();
    }

    public void ConfirmButton()
    {
        SaveDataManager.saveData.name = nameInput.text;
        SaveDataManager.saveData.pronouns = pronouns[pronounsIdx].pronounInDialogue;
        if (dialogueInputField.gameObject.activeSelf)
            SaveDataManager.saveData.currentDialogueID = dialogueInputField.text;
        else
        {
            devInputAction.Disable();
            devInputAction.performed -= DevShowInputField;
        }
        FadeManager.StartFade(false, LoadGame, Color.black);
        AudioManager.FadeMusicOut();
    }

    public void BackButton()
    {
        mainMenu.Show(false);
        charCreatorCanvas.gameObject.SetActive(false);
        devInputAction.Disable();
        devInputAction.performed -= DevShowInputField;
    }

    public void LoadGame() => SceneManager.LoadScene(gameSceneName);

    private void DevShowInputField(InputAction.CallbackContext context)
    {
        devInputAction.Disable();
        devInputAction.performed -= DevShowInputField;
        dialogueInputField.text = string.Empty;
        dialogueInputField.gameObject.SetActive(true);
    }

    public void DevInputFieldEnter()
    {
        invalidDialogueID = true;
        foreach (Dialogue dialogue in dialogueHolder.dialogues)
        {
            if (dialogue.name != dialogueInputField.text) continue;
            invalidDialogueID = false;
            break;
        }
        UpdateName();
    }
}