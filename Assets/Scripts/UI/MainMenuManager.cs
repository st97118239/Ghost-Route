using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;
    [SerializeField] private Transform eyeTrans;
    [SerializeField] private Vector3 eyeClampLeftTop;
    [SerializeField] private Vector3 eyeClampRightBottom;

    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private CharCreatorManager charCreatorManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private EndingsManager endingsManager;

    [SerializeField] private string gameSceneName;

    public static Ending[] endings;

    [SerializeField] private Ending[] _endings;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private MainMenuBackground backgroundChanger;

    private bool shouldMoveEye;

    private void Awake()
    {
        FadeManager.Show();
        endings = _endings;
        Show();

        if (SaveDataManager.saveData == null)
            SaveDataManager.LookForSave();

        startButtonText.text = SaveDataManager.saveData.currentDialogueID == string.Empty ? "Start" : "Continue";
    }

    private void Start()
    {
        FadeManager.StartFade(true, null, Color.black);
        AudioManager.FadeMusicIn(Sounds.MainMusic);
    }

    private IEnumerator MoveEyeLoop()
    {
        shouldMoveEye = true;
        eyeTrans.localPosition = Vector3.zero;

        while (shouldMoveEye)
        {
            Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 centeredMousePos = new(mousePos.x + 0.08f, mousePos.y - 0.22f);

            float x = Mathf.Lerp(eyeClampLeftTop.x, eyeClampRightBottom.x, centeredMousePos.x);
            float y = Mathf.Lerp(eyeClampLeftTop.y, eyeClampRightBottom.y, centeredMousePos.y);
            float z = centeredMousePos.z;

            eyeTrans.localPosition = new(x, y, z);

            yield return null;
        }
    }

    private IEnumerator MoveEyeToMiddle()
    {
        shouldMoveEye = false;

        Vector2 eyePos = new(eyeTrans.localPosition.x, eyeTrans.localPosition.y);

        for (float i = 0; i < 1 + Time.deltaTime; i += Time.deltaTime)
        {
            float x = Mathf.Lerp(eyePos.x, 0, i);
            float y = Mathf.Lerp(eyePos.y, 0, i);
            const float z = 0;

            eyeTrans.localPosition = new(x, y, z);
            yield return Time.deltaTime;
        }

        eyeTrans.localPosition = Vector3.zero;
    }

    public void Show()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        backgroundChanger.ChangeBackground(true);
        StartCoroutine(MoveEyeLoop());
    }

    public void StartButton()
    {
        if (SaveDataManager.saveData.currentDialogueID == string.Empty)
            ShowCharCreator();
        else
        {
            FadeManager.StartFade(false, charCreatorManager.LoadGame, Color.black);
            AudioManager.FadeMusicOut();
            StartCoroutine(MoveEyeToMiddle());
        }
    }

    public void ShowCharCreator()
    {
        charCreatorManager.Show();
        shouldMoveEye = false;
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void EndingsButton()
    {
        endingsManager.Show();
        shouldMoveEye = false;
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void SettingsButton()
    {
        settingsManager.Show();
        shouldMoveEye = false;
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void QuitButton()
    {
        StartCoroutine(MoveEyeToMiddle());
        backgroundChanger.ChangeBackground(false);
        SaveDataManager.Save();
        AudioManager.FadeMusicOut();
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
