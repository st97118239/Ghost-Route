using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingsManager : MonoBehaviour
{
    [SerializeField] private Canvas endingsCanvas;
    [SerializeField] private MainMenuManager mainMenu;

    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Image badgeImage;
    [SerializeField] private TMP_Text endingName;
    [SerializeField] private TMP_Text endingDescription;

    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_Text[] buttonsText;

    private int endingIdx;

    private void Start()
    {
        endingIdx = -1;
        if (MainMenuManager.endings.Length > buttonsText.Length)
        {
            Debug.LogWarning("Only " + buttonsText.Length + " endings allowed");
        }

        for (int i = 0; i < buttonsText.Length; i++)
        {
            Ending ending = MainMenuManager.endings[i];
            if (PlayerPrefs.GetInt(ending.endingID) == 1)
            {
                buttonsText[i].text = MainMenuManager.endings[i].endingName;
                buttons[i].interactable = true;
            }
            else
            {
                buttonsText[i].text = "???";
                buttons[i].interactable = false;
            }
        }
    }

    public void Show()
    {
        endingsCanvas.gameObject.SetActive(true);
    }

    public void SelectEnding(int idx)
    {
        endingIdx = idx;
        LoadCard();
    }

    private void LoadCard()
    {
        Ending selectedEnding = MainMenuManager.endings[endingIdx];
        badgeImage.sprite = selectedEnding.badgeSprite;
        endingName.text = selectedEnding.endingName;
        endingDescription.text = selectedEnding.description;
        cardPanel.SetActive(true);
    }

    public void BackButton()
    {
        mainMenu.Show();
        endingsCanvas.gameObject.SetActive(false);
    }
}
