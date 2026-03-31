using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GhostHuntManager : MonoBehaviour
{
    [SerializeField] private Transform targetParent;
    private TargetObj[] targetObjs;
    private List<TargetObj> disabledTargets;
    private int targetsActive;

    [SerializeField] private int maxTargetsOntAtOnce;
    [SerializeField] private int startingMinAmt;
    [SerializeField] private int startingMaxAmt;

    [SerializeField] private int time;
    private int timeLeft;
    [SerializeField] private int canSpawnUntil;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text pointText;

    [SerializeField] private int spawnChance;
    [SerializeField] private float minSecondsBetweenSpawns;
    [SerializeField] private float devMinSecondsBetweenSpawns;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;

    private int points;
    [SerializeField] private int maxPoints;

    private int timeSinceLastSpawn;

    [SerializeField] private Transform cursor;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Color baseCursorColor;
    [SerializeField] private Color shootCursorColor;
    [SerializeField] private float shootCursorTime;
    private WaitForSeconds shootCursorTimeWait;
    private Coroutine shootCursorCoroutine;

    [SerializeField] private AudioClip beginVoiceline;

    private bool isPlaying;

    private void Awake()
    {
        FadeManager.Show();

        targetObjs = new TargetObj[targetParent.childCount];
        for (int i = 0; i < targetParent.childCount; i++)
            targetObjs[i] = targetParent.GetChild(i).GetComponent<TargetObj>();

        shootCursorTimeWait = new WaitForSeconds(shootCursorTime);
        cursorImage.color = baseCursorColor;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
    }

    private void Start()
    {
        for (int i = 0; i < targetObjs.Length; i++) 
            targetObjs[i].Setup(this, i);

        if (startingMinAmt > targetObjs.Length) startingMinAmt = targetObjs.Length;
        if (startingMinAmt > maxTargetsOntAtOnce) startingMinAmt = maxTargetsOntAtOnce;

        FadeManager.StartFade(true, LoadGame, Color.black);
        AudioManager.FadeMusicIn(Sounds.MainMusic);
    }

    private void LoadGame()
    {
        StartCoroutine(PlayVoicelines());
    }

    private IEnumerator PlayVoicelines()
    {
        yield return new WaitForSeconds(1);

        float delay = AudioManager.PlayVoiceline(beginVoiceline);
        yield return new WaitForSeconds(delay);

        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(SpawnLoop());

        devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
        devInputAction.performed += DevCheat;
        devInputAction.Enable();
    }

    public void Shoot(int amt)
    {
        points += amt;
        if (points < 0)
            points = 0;
        pointText.text = points.ToString();
        ShootEffect();
    }

    public void ShootEffect()
    {
        AudioManager.PlaySound(Sounds.Shoot, false);
        if (shootCursorCoroutine != null)
            StopCoroutine(shootCursorCoroutine);
        shootCursorCoroutine = StartCoroutine(CursorShootEvent());
    }

    private IEnumerator CursorShootEvent()
    {
        cursorImage.color = baseCursorColor;
        yield return null;
        cursorImage.color = shootCursorColor;

        yield return shootCursorTimeWait;

        cursorImage.color = baseCursorColor;
    }

    public void Hide(int idx)
    {
        disabledTargets.Add(targetObjs[idx]);
        timeSinceLastSpawn = 0;
        targetsActive--;
    }

    private IEnumerator SpawnLoop()
    {
        isPlaying = true;
        disabledTargets = targetObjs.ToList();
        int max = startingMaxAmt;
        if (max == 0 && targetObjs.Length > 0)
            max = 1;
        if (max > targetObjs.Length)
            max = targetObjs.Length;
        if (max == 0 || targetObjs.Length == 0)
        {
            Debug.LogError("There are no target objects");
            yield break;
        }

        StartCoroutine(FollowCursor());

        WaitForSeconds wait1Sec = new(1);

        timeText.text = timeLeft.ToString();
        yield return wait1Sec;

        int amt = Random.Range(startingMinAmt, max);
        for (int i = 0; i < amt; i++)
        {
            TargetObj obj = targetObjs[i];
            obj.Show();
            disabledTargets.Remove(obj);
        }
        targetsActive = amt;

        timeSinceLastSpawn = 0;
        for (timeLeft = time; timeLeft > -1; timeLeft -= 1)
        {
            timeText.text = timeLeft.ToString();
            timeSinceLastSpawn++;

            if (timeLeft > canSpawnUntil && targetsActive < maxTargetsOntAtOnce && (Random.Range(0, 100) <= spawnChance || timeSinceLastSpawn >= minSecondsBetweenSpawns))
            {
                if (disabledTargets.Count > 0)
                {
                    int i = Random.Range(0, disabledTargets.Count);
                    disabledTargets[i].Show();
                    disabledTargets.RemoveAt(i);
                    targetsActive++;
                    timeSinceLastSpawn = 0;
                }
            }

            yield return wait1Sec;
        }

        if (targetsActive > 0)
        {
            foreach (TargetObj obj in targetObjs)
            {
                if (disabledTargets.Contains(obj)) continue;

                obj.Leave();
            }

            yield return wait1Sec;
        }

        isPlaying = false;

        yield return wait1Sec;

        EndGame();
    }

    private IEnumerator FollowCursor()
    {
        Cursor.visible = false;
        cursor.gameObject.SetActive(true);

        while (isPlaying)
        {
            cursor.position = Input.mousePosition;
            yield return null;
        }

        cursor.gameObject.SetActive(false);
        Cursor.visible = true;
    }

    private void EndGame()
    {
        devInputAction.Disable();
        devInputAction.performed -= DevCheat;

#if UNITY_EDITOR
        if (SaveDataManager.saveData == null)
        {
            FadeManager.StartFade(false, null, Color.black);
            AudioManager.FadeMusicOut();
            return;
        }
#endif

        if (points >= maxPoints)
        {
            SaveDataManager.saveData.hasPlayedGhostHunt = true;
            SaveDataManager.saveData.ghostHuntScore = points;
            FadeManager.StartFade(false, ExitGame, Color.black);
        }
        else
            FadeManager.StartFade(false, RestartGame, Color.black);

        AudioManager.FadeMusicOut();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private static void ExitGame()
    {
        SceneManager.LoadScene("Dialogue");
    }

    private static void RestartGame()
    {
        SceneManager.LoadScene("Ghost Hunt");
    }

    private void DevCheat(InputAction.CallbackContext context)
    {
        foreach (TargetObj target in targetObjs) 
            target.DevCheat();

        minSecondsBetweenSpawns = devMinSecondsBetweenSpawns;
        spawnChance = 100;
    }
}
