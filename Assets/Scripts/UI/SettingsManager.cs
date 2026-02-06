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
    [SerializeField] private float maxTextSpeed;

    [SerializeField] private GameObject windowTypePanel;
    [SerializeField] private int windowTypeIdx;

    [SerializeField] private GameObject bgmVolumePanel;
    [SerializeField] private Slider bgmVolumeSlider;

    [SerializeField] private GameObject sfxVolumePanel;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private GameObject voicelineVolumePanel;
    [SerializeField] private Slider voicelineVolumeSlider;

    private void Start()
    {
        if (PlayerPrefs.GetInt("NeedsSettings") == 0)
        {
            PlayerPrefs.SetInt("NeedsSettings", 1);
            PlayerPrefs.SetFloat("TextSpeed", 0.5f);
            PlayerPrefs.SetInt("WindowType", 0);
            PlayerPrefs.SetFloat("BGMVolume", 1);
            PlayerPrefs.SetFloat("SFXVolume", 1);
            PlayerPrefs.SetFloat("VoicelinesVolume", 1);
        }

        textSpeedSlider.maxValue = maxTextSpeed;
        textSpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 0.5f);
        windowTypeIdx = PlayerPrefs.GetInt("WindowType", 0);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        voicelineVolumeSlider.value = PlayerPrefs.GetFloat("VoicelinesVolume", 1);
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
            PlayerPrefs.SetFloat("TextSpeed", textSpeedSlider.value);
        }
        else
            textSpeedPanel.SetActive(true);
    }

    public void WindowTypeButton()
    {
        if (windowTypePanel.activeSelf)
        {
            windowTypePanel.SetActive(false);
            PlayerPrefs.SetInt("WindowType", windowTypeIdx);
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
    }

    public void BGMButton()
    {
        if (bgmVolumePanel.activeSelf)
        {
            bgmVolumePanel.SetActive(false);
            PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value);
        }
        else
            bgmVolumePanel.SetActive(true);
    }

    public void SFXButton()
    {
        if (sfxVolumePanel.activeSelf)
        {
            sfxVolumePanel.SetActive(false);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        }
        else
            sfxVolumePanel.SetActive(true);
    }

    public void VoicelinesButton()
    {
        if (voicelineVolumePanel.activeSelf)
        {
            voicelineVolumePanel.SetActive(false);
            PlayerPrefs.SetFloat("VoicelinesVolume", voicelineVolumeSlider.value);
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
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void CancelResetButton()
    {
        resetConfirmPanel.SetActive(false);
    }

    public void BackButton()
    {
        mainMenu.Show();
        settingsCanvas.gameObject.SetActive(false);
    }
}