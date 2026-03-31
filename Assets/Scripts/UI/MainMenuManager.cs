using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;

    [SerializeField] private Transform eye0Trans;
    [SerializeField] private Transform eye1Trans;
    [SerializeField] private Transform eye2Trans;
    [SerializeField] private Vector3 eye0ClampLeftTop;
    [SerializeField] private Vector3 eye0ClampRightBottom;
    [SerializeField] private Vector3 eye1ClampLeftTop;
    [SerializeField] private Vector3 eye1ClampRightBottom;
    [SerializeField] private Vector3 eye2ClampLeftTop;
    [SerializeField] private Vector3 eye2ClampRightBottom;
    private List<Vector3> cursorPositions;
    [SerializeField] private int savedCursorPositionsAmt;

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
        Show(true);

        if (SaveDataManager.saveData == null)
            SaveDataManager.LookForSave();

        startButtonText.text = SaveDataManager.saveData.currentDialogueID == string.Empty ? "Start" : "Continue";

        eye0Trans.localPosition = Vector3.zero;
        eye1Trans.localPosition = Vector3.zero;
        eye2Trans.localPosition = Vector3.zero;
    }

    private void Start()
    {
        FadeManager.StartFade(true, ShowMoon, Color.black);
        AudioManager.FadeMusicIn(Sounds.MainMusic);
    }

    private void ShowMoon() => backgroundChanger.ChangeBackground(true);

    private IEnumerator MoveEyeLoop()
    {
        shouldMoveEye = true;

        cursorPositions = new List<Vector3>();
        for (int i = 0; i < savedCursorPositionsAmt; i++)
        {
            Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 centeredMousePos = new(mousePos.x + 0.08f, mousePos.y - 0.22f);
            cursorPositions.Add(centeredMousePos);
            yield return null;
        }

        while (shouldMoveEye)
        {
            Vector3 mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 centeredMousePos = new(mousePos.x + 0.08f, mousePos.y - 0.22f);

            cursorPositions.Add(centeredMousePos);

            centeredMousePos = cursorPositions[0];

            float x = Mathf.Lerp(eye0ClampLeftTop.x, eye0ClampRightBottom.x, centeredMousePos.x);
            float y = Mathf.Lerp(eye0ClampLeftTop.y, eye0ClampRightBottom.y, centeredMousePos.y);
            float z = centeredMousePos.z;

            eye0Trans.localPosition = new(x, y, z);

            x = Mathf.Lerp(eye1ClampLeftTop.x, eye1ClampRightBottom.x, centeredMousePos.x);
            y = Mathf.Lerp(eye1ClampLeftTop.y, eye1ClampRightBottom.y, centeredMousePos.y);

            eye1Trans.localPosition = new(x, y, z);

            x = Mathf.Lerp(eye2ClampLeftTop.x, eye2ClampRightBottom.x, centeredMousePos.x);
            y = Mathf.Lerp(eye2ClampLeftTop.y, eye2ClampRightBottom.y, centeredMousePos.y);

            eye2Trans.localPosition = new(x, y, z);

            cursorPositions.RemoveAt(0);

            yield return null;
        }
    }

    private IEnumerator MoveEyeToMiddle()
    {
        shouldMoveEye = false;

        Vector2 eye0Pos = new(eye0Trans.localPosition.x, eye0Trans.localPosition.y);
        Vector2 eye1Pos = new(eye1Trans.localPosition.x, eye1Trans.localPosition.y);
        Vector2 eye2Pos = new(eye2Trans.localPosition.x, eye2Trans.localPosition.y);

        for (float i = 0; i < 1 + Time.deltaTime; i += Time.deltaTime)
        {
            float x = Mathf.Lerp(eye0Pos.x, 0, i);
            float y = Mathf.Lerp(eye0Pos.y, 0, i);
            const float z = 0;

            eye0Trans.localPosition = new(x, y, z);

            x = Mathf.Lerp(eye1Pos.x, 0, i);
            y = Mathf.Lerp(eye1Pos.y, 0, i);

            eye1Trans.localPosition = new(x, y, z);

            x = Mathf.Lerp(eye2Pos.x, 0, i);
            y = Mathf.Lerp(eye2Pos.y, 0, i);

            eye2Trans.localPosition = new(x, y, z);

            yield return Time.deltaTime;
        }

        eye0Trans.localPosition = Vector3.zero;
        eye1Trans.localPosition = Vector3.zero;
        eye2Trans.localPosition = Vector3.zero;
    }

    public void Show(bool isFromStart)
    {
        mainMenuCanvas.gameObject.SetActive(true);
        if (!isFromStart)
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
