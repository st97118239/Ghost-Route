using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TargetObj : MonoBehaviour
{
    private GhostHuntManager manager;

    [SerializeField] private TargetsHolder targetsHolder;
    [SerializeField] private int ghostChance;
    [SerializeField] private int bunnyChance;
    [SerializeField] private int deerChance;
    private Target currentTarget;
    private int totalChance;

    [SerializeField] private Image image;
    [SerializeField] private Button button;

    [SerializeField] private Animator animator;
    [SerializeField] private int animIdx;

    [SerializeField] private float _timeToDisappear;
    private WaitForSeconds timeToDisappear;
    private Coroutine disappearCoroutine;

    private int idx;

    public bool isActive { get; private set; }

    private void Awake()
    {
        image.enabled = false;
        button.enabled = false;
        timeToDisappear = new WaitForSeconds(_timeToDisappear);
        totalChance = ghostChance + bunnyChance + deerChance;
    }

    public void Setup(GhostHuntManager givenManager, int givenIdx)
    {
        manager = givenManager;
        idx = givenIdx;
    }

    public void Show()
    {
        int chance = Random.Range(0, totalChance);
        if (chance < ghostChance)
        {
            currentTarget = targetsHolder.ghosts[Random.Range(0, targetsHolder.ghosts.Length)];
            AudioManager.PlaySound(Sounds.SpawnGhost, true);
        }
        else if (chance > ghostChance && chance < ghostChance + bunnyChance)
        {
            currentTarget = targetsHolder.bunnies[Random.Range(0, targetsHolder.bunnies.Length)];
            AudioManager.PlaySound(Sounds.SpawnBunny, true);
        }
        else if (chance > ghostChance + bunnyChance)
        {
            currentTarget = targetsHolder.deer[Random.Range(0, targetsHolder.deer.Length)];
            AudioManager.PlaySound(Sounds.SpawnDeer, true);
        }

        image.sprite = currentTarget.sprite;
        image.enabled = true;
        button.enabled = true;
        isActive = true;
        animator.SetInteger("Move", animIdx);
        disappearCoroutine = StartCoroutine(Disappear(true));
    }

    public void Shoot()
    {
        AudioManager.instance.PlayShoot();
        AudioManager.PlaySound(currentTarget.type == TargetType.Ghost ? Sounds.Hit : Sounds.Wrong, true);
        if (disappearCoroutine != null)
            StopCoroutine(disappearCoroutine);
        image.enabled = false;
        disappearCoroutine = StartCoroutine(Disappear(false));
        manager.Shoot(currentTarget.points);
    }

    private void Hide()
    {
        button.enabled = false;
        animator.SetInteger("Move", -1);
        disappearCoroutine = null;
    }

    private IEnumerator Disappear(bool extraWait)
    {
        if (extraWait)
            yield return timeToDisappear;

        Hide();

        yield return new WaitForSeconds(0.5f);

        isActive = false;
        image.enabled = false;
        manager.Hide(idx);
    }

    public void Leave()
    {
        if (disappearCoroutine != null)
            StopCoroutine(disappearCoroutine);
        disappearCoroutine = StartCoroutine(Disappear(false));
    }

    public void DevCheat()
    {
        if (ghostChance > 0)
            ghostChance = 100;
        deerChance = 0;
        bunnyChance = 0;
    }
}
