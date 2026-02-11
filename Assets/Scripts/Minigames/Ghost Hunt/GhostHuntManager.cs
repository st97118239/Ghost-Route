#if UNITY_EDITOR
using System.Linq; // LINQ IS ONLY IN EDITOR
#endif
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private int maxSecondsBetweenSpawns;

    private int points;
    [SerializeField] private int maxPoints;

    private int timeSinceLastSpawn;

    [SerializeField] private Transform cursor;

    private bool isPlaying;

    private void Start()
    {
#if UNITY_EDITOR
        TargetObj[] objs = FindObjectsByType<TargetObj>(FindObjectsSortMode.None);
        foreach (TargetObj obj in objs)
        {
            if (targetObjs.Contains(obj)) continue;
            
            Debug.LogWarning(obj.gameObject.name + " is not in targetObjs");
        }
#endif

        for (int i = 0; i < targetObjs.Length; i++) 
            targetObjs[i].Setup(this, i);

        if (startingMinAmt > targetObjs.Length) startingMinAmt = targetObjs.Length;
        if (startingMinAmt > maxTargetsOntAtOnce) startingMinAmt = maxTargetsOntAtOnce;
        StartCoroutine(SpawnLoop());
    }

    public void Shoot(int amt)
    {
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

            if (timeLeft > canSpawnUntil && targetsActive < maxTargetsOntAtOnce && (Random.Range(0, 100) <= spawnChance || timeSinceLastSpawn >= maxSecondsBetweenSpawns))
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
        SaveData.hasPlayedGhostHunt = true;
        SaveData.ghostHuntScore = points;
        SceneManager.LoadScene("Dialogue");
    }
}
