using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;

    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private EndingsManager endingsManager;

    [SerializeField] private string gameSceneName;

    public static Ending[] endings;

    [SerializeField] private Ending[] _endings;

    [SerializeField] private bool shouldReset;

    private void Awake()
    {
        endings = _endings;

        if (shouldReset)
            SaveData.ResetData();


        SaveData.LoadSave();

        startButtonText.text = SaveData.currentDialogueID == string.Empty ? "Start" : "Continue";
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
        endingsManager.Show();
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void SettingsButton()
    {
        settingsManager.Show();
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void QuitButton()
    {
        SaveData.Save();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
