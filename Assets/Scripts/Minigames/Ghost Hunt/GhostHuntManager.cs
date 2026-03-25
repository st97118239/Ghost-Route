using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class GhostHuntManager : MonoBehaviour
{
    [SerializeField] private TargetObj[] targetObjs;
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

    [SerializeField] private AudioClip[] beginVoicelines;

    private bool isPlaying;

    private void Start()
    {
        FadeManager.Show();

        for (int i = 0; i < targetObjs.Length; i++) 
            targetObjs[i].Setup(this, i);

        if (startingMinAmt > targetObjs.Length) startingMinAmt = targetObjs.Length;
        if (startingMinAmt > maxTargetsOntAtOnce) startingMinAmt = maxTargetsOntAtOnce;

        FadeManager.StartFade(true, LoadGame, Color.black);
    }

    private void LoadGame()
    {
        AudioManager.PlaySound(Sounds.Music, false);
        StartCoroutine(PlayVoicelines());
    }

    private IEnumerator PlayVoicelines()
    {
        yield return new WaitForSeconds(1);

        if (beginVoicelines != null && beginVoicelines.Length > 0)
        {
            foreach (AudioClip voiceline in beginVoicelines)
            {
                float delay = AudioManager.PlayVoiceline(voiceline);

                yield return new WaitForSeconds(delay);
            }
        }

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
        AudioManager.PlaySound(Sounds.Shoot, false);
        points += amt;
        pointText.text = points.ToString();
        if (points >= maxPoints) 
            timeLeft = 0;
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
                int i = Random.Range(0, disabledTargets.Count);
                disabledTargets[i].Show();
                disabledTargets.RemoveAt(i);
                targetsActive++;
                timeSinceLastSpawn = 0;
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
        AudioManager.PlaySound(Sounds.Ending, false);

        if (points >= maxPoints)
        {
            SaveDataManager.saveData.hasPlayedGhostHunt = true;
            SaveDataManager.saveData.ghostHuntScore = points;
            FadeManager.StartFade(false, ExitGame, Color.black);
        }
        else
        {
            FadeManager.StartFade(false, RestartGame, Color.black);
        }
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
        {
            target.DevCheat();
        }

        minSecondsBetweenSpawns = devMinSecondsBetweenSpawns;
        spawnChance = 100;
    }
}
