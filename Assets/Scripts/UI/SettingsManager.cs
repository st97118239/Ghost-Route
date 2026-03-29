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

    [SerializeField] private GameObject bgmVolumePanel;
    [SerializeField] private Slider bgmVolumeSlider;

    [SerializeField] private GameObject sfxVolumePanel;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private GameObject voicelineVolumePanel;
    [SerializeField] private Slider voicelineVolumeSlider;

    private bool isSettingUp;

    private void Start()
    {
        isSettingUp = true;
        textSpeedSlider.value = textSpeedSlider.maxValue - SaveDataManager.saveData.textSpeed;
        windowTypeIdx = SaveDataManager.saveData.windowType;
        bgmVolumeSlider.value = SaveDataManager.saveData.bgmVolume;
        sfxVolumeSlider.value = SaveDataManager.saveData.sfxVolume;
        voicelineVolumeSlider.value = SaveDataManager.saveData.voicelinesVolume;
        SetWindowType();
        isSettingUp = false;
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
            float textSpeedValue = textSpeedSlider.maxValue - textSpeedSlider.value;
            Debug.Log(textSpeedValue);
            SaveDataManager.saveData.textSpeed = textSpeedValue;
        }
        else
            textSpeedPanel.SetActive(true);
    }

    public void WindowTypeButton()
    {
        if (windowTypePanel.activeSelf)
        {
            windowTypePanel.SetActive(false);
            SaveDataManager.saveData.windowType = windowTypeIdx;
        }
        else
        {
            windowTypeIdx = Screen.fullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => 0,
                FullScreenMode.FullScreenWindow => 1,
                FullScreenMode.MaximizedWindow => 2,
                FullScreenMode.Windowed => 3,
                _ => 1
            };

            SetWindowType();

            windowTypePanel.SetActive(true);
        }
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

    public void BGMButton()
    {
        bgmVolumePanel.SetActive(!bgmVolumePanel.activeSelf);
    }

    public void SFXButton()
    {
        sfxVolumePanel.SetActive(!sfxVolumePanel.activeSelf);
    }

    public void VoicelinesButton()
    {
        voicelineVolumePanel.SetActive(!voicelineVolumePanel.activeSelf);
    }

    public void UpdateAudio()
    {
        if (isSettingUp) return;
        SaveDataManager.saveData.bgmVolume = bgmVolumeSlider.value;
        SaveDataManager.saveData.sfxVolume = sfxVolumeSlider.value;
        SaveDataManager.saveData.voicelinesVolume = voicelineVolumeSlider.value;
        AudioManager.SetVolumes();
    }

    public void ResetDataButton()
    {
        resetConfirmPanel.SetActive(true);
    }

    public void ConfirmResetButton()
    {
        SaveDataManager.ResetData();
        FadeManager.StartFade(false, ReloadScene, Color.black);
        AudioManager.FadeMusicOut();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void CancelResetButton()
    {
        resetConfirmPanel.SetActive(false);
    }

    public void BackButton()
    {
        SaveDataManager.Save();
        mainMenu.Show();
        settingsCanvas.gameObject.SetActive(false);
    }
}