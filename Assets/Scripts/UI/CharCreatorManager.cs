using TMPro;
using UnityEngine;
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

    private void Awake()
    {
        pronounsIdx = 1;
    }

    public void Show()
    {
        charCreatorCanvas.gameObject.SetActive(true);

        for (int i = 0; i < pronounsCheckmarks.Length; i++)
            pronounsCheckmarks[i].gameObject.SetActive(i == pronounsIdx);

        confirmButton.interactable = nameInput.text != string.Empty;
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

    public void UpdateName()
    {
        confirmButton.interactable = nameInput.text != string.Empty;
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
        SaveData.name = nameInput.text;
        SaveData.pronouns = pronouns[pronounsIdx].pronounInDialogue;
        SceneManager.LoadScene(gameSceneName);
    }

    public void BackButton()
    {
        mainMenu.Show();
        charCreatorCanvas.gameObject.SetActive(false);
    }
}