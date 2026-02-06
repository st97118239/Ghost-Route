using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private MainMenuManager mainMenu;
    [SerializeField] private Canvas settingsCanvas;

    [SerializeField] private GameObject resetConfirmPanel;
    [SerializeField] private string mainMenuSceneName;

    [SerializeField] private GameObject textSpeedPanel;
    [SerializeField] private Slider textSpeedSlider;

    [SerializeField] private GameObject windowTypePanel;
    [SerializeField] private int windowTypeIdx;
    [SerializeField] private Image[] windowTypeCheckmarks;

    [SerializeField] private Toggle showGoreToggle;

    [SerializeField] private GameObject bgmVolumePanel;
    [SerializeField] private Slider bgmVolumeSlider;

    [SerializeField] private GameObject sfxVolumePanel;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private GameObject voicelineVolumePanel;
    [SerializeField] private Slider voicelineVolumeSlider;

    private void Start()
    {
        textSpeedSlider.value = 1 - SaveData.textSpeed;
        windowTypeIdx = SaveData.windowType;
        bgmVolumeSlider.value = SaveData.bgmVolume;
        sfxVolumeSlider.value = SaveData.sfxVolume;
        voicelineVolumeSlider.value = SaveData.voicelinesVolume;
        showGoreToggle.isOn = SaveData.showGore;
        SetWindowType();
    }

    public void Show()
    {
        settingsCanvas.gameObject.SetActive(true);
    }

    public void CloseAllButtons(int idxToKeep)
    {
        if (idxToKeep != 0 && textSpeedPanel.activeSelf)
            TextSpeedButton();
        if (idxToKeep != 1 && windowTypePanel.activeSelf)
            WindowTypeButton();
        if (idxToKeep != 2 && bgmVolumePanel.activeSelf)
            BGMButton();
        if (idxToKeep != 3 && sfxVolumePanel.activeSelf)
            SFXButton();
        if (idxToKeep != 4 && voicelineVolumePanel.activeSelf)
            VoicelinesButton();
    }

    public void TextSpeedButton()
    {
        if (textSpeedPanel.activeSelf)
        {
            textSpeedPanel.SetActive(false);
            SaveData.textSpeed = 1 - textSpeedSlider.value;
        }
        else
            textSpeedPanel.SetActive(true);
    }

    public void WindowTypeButton()
    {
        if (windowTypePanel.activeSelf)
        {
            windowTypePanel.SetActive(false);
            SaveData.windowType = windowTypeIdx;
        }
        else
            windowTypePanel.SetActive(true);
    }

    public void WindowTypeButton(int idx)
    {
        windowTypeIdx = idx;

        SetWindowType();

        WindowTypeButton();
    }

    private void SetWindowType()
    {
        Screen.fullScreenMode = windowTypeIdx switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.MaximizedWindow,
            3 => FullScreenMode.Windowed,
            _ => Screen.fullScreenMode
        };

        for (int i = 0; i < windowTypeCheckmarks.Length; i++) 
            windowTypeCheckmarks[i].gameObject.SetActive(i == windowTypeIdx);
    }

    public void ShowGoreToggle()
    {
        SaveData.showGore = showGoreToggle.isOn;
    }

    public void BGMButton()
    {
        if (bgmVolumePanel.activeSelf)
        {
            bgmVolumePanel.SetActive(false);
            SaveData.bgmVolume = bgmVolumeSlider.value;
        }
        else
            bgmVolumePanel.SetActive(true);
    }

    public void SFXButton()
    {
        if (sfxVolumePanel.activeSelf)
        {
            sfxVolumePanel.SetActive(false);
            SaveData.sfxVolume = sfxVolumeSlider.value;
        }
        else
            sfxVolumePanel.SetActive(true);
    }

    public void VoicelinesButton()
    {
        if (voicelineVolumePanel.activeSelf)
        {
            voicelineVolumePanel.SetActive(false);
            SaveData.voicelinesVolume = voicelineVolumeSlider.value;
        }
        else
            voicelineVolumePanel.SetActive(true);
    }

    public void ResetDataButton()
    {
        resetConfirmPanel.SetActive(true);
    }

    public void ConfirmResetButton()
    {
        SaveData.ResetData();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void CancelResetButton()
    {
        resetConfirmPanel.SetActive(false);
    }

    public void BackButton()
    {
        SaveData.Save();
        mainMenu.Show();
        settingsCanvas.gameObject.SetActive(false);
    }
}