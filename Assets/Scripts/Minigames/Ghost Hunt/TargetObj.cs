using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TargetObj : MonoBehaviour
{
    private GhostHuntManager manager;

    [SerializeField] private TargetsHolder targetsHolder;
    [SerializeField] private TargetType target;
    private Target currentTarget;

    [SerializeField] private Image image;
    [SerializeField] private Button button;

    [SerializeField] private Animator animator;
    [SerializeField] private int animIdx;

    [SerializeField] private float timeToDisappear = 3;
    private WaitForSeconds timeToDisappearWait;
    [SerializeField] private float animationTime = 0.5f;
    private WaitForSeconds animationTimeWait;
    [SerializeField] private bool shouldReturn = true;
    private Coroutine disappearCoroutine;

    private int idx;

    public bool isActive { get; private set; }

    private void Awake()
    {
        image.enabled = false;
        button.enabled = false;
        animationTimeWait = new WaitForSeconds(animationTime);
        if (shouldReturn)
            timeToDisappearWait = new WaitForSeconds(timeToDisappear);
    }

    public void Setup(GhostHuntManager givenManager, int givenIdx)
    {
        manager = givenManager;
        idx = givenIdx;
    }

    public void Show()
    {
        switch (target)
        {
            default:
            case TargetType.None:
                return;
            case TargetType.Ghost:
                currentTarget = targetsHolder.ghosts[Random.Range(0, targetsHolder.ghosts.Length)];
                AudioManager.PlaySound(Sounds.SpawnGhost, true);
                break;
            case TargetType.Bunny:
                currentTarget = targetsHolder.bunnies[Random.Range(0, targetsHolder.bunnies.Length)];
                AudioManager.PlaySound(Sounds.SpawnBunny, true);
                break;
            case TargetType.Deer:
                currentTarget = targetsHolder.deer[Random.Range(0, targetsHolder.deer.Length)];
                AudioManager.PlaySound(Sounds.SpawnDeer, true);
                break;
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
        if (shouldReturn)
        {
            if (extraWait)
            {
                yield return animationTimeWait;
                yield return timeToDisappearWait;
            }

            Hide();

            yield return animationTimeWait;
        }
        else
        {
            if (extraWait)
                yield return animationTimeWait;

            Hide();
        }

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

    public void DevCheat() => target = TargetType.Ghost;
}
