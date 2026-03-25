using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;

    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private CharCreatorManager charCreatorManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private EndingsManager endingsManager;

    [SerializeField] private string gameSceneName;

    public static Ending[] endings;

    [SerializeField] private Ending[] _endings;

    private void Awake()
    {
        FadeManager.Show();
        endings = _endings;

        if (SaveDataManager.saveData == null)
            SaveDataManager.LookForSave();

        startButtonText.text = SaveDataManager.saveData.currentDialogueID == string.Empty ? "Start" : "Continue";
    }

    private void Start()
    {
        AudioManager.PlaySound(Sounds.Music, false);
        FadeManager.StartFade(true, null, Color.black);
    }

    public void Show()
    {
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        if (SaveDataManager.saveData.currentDialogueID == string.Empty)
            ShowCharCreator();
        else
            FadeManager.StartFade(false, charCreatorManager.LoadGame, Color.black);
    }

    public void ShowCharCreator()
    {
        charCreatorManager.Show();
        mainMenuCanvas.gameObject.SetActive(false);
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
        SaveDataManager.Save();
        FadeManager.StartFade(false, Quit, Color.black);
    }

    private static void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
