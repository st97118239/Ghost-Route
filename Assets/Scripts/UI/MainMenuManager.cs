using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;

    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private SettingsManager settingsManager;

    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        startButtonText.text = PlayerPrefs.GetString("DialogueID") == string.Empty ? "Start" : "Continue";
    }

    public void Show()
    {
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void EndingsButton()
    {

    }

    public void SettingsButton()
    {
        settingsManager.Show();
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
