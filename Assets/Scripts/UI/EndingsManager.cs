using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;

    private int endingIdx;

    private void Start()
    {
        endingIdx = -1;
        if (MainMenuManager.endings.Length > buttonsText.Length)
        {
            Debug.LogWarning("Only " + buttonsText.Length + " endings allowed");
        }

        LoadEndings();

        devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
    }

    private void LoadEndings()
    {
        for (int i = 0; i < buttonsText.Length; i++)
        {
            Ending ending = MainMenuManager.endings[i];
            if (SaveData.endings[ending.endingID].isUnlocked)
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
        AudioManager.PlaySound(Sounds.Ending);
        devInputAction.performed += DevUnlockAllEndings;
        devInputAction.Enable();
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
        devInputAction.Disable();
        devInputAction.performed -= DevUnlockAllEndings;
    }

    private void DevUnlockAllEndings(InputAction.CallbackContext context)
    {
        foreach (KeyValuePair<string, Ending> ending in SaveData.endings)
            ending.Value.isUnlocked = true;

        devInputAction.Disable();
        devInputAction.performed -= DevUnlockAllEndings;
        LoadEndings();
    }
}
