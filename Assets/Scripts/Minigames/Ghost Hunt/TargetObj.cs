using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class TargetObj : MonoBehaviour
{
    private GhostHuntManager manager;

    [SerializeField] private Target[] ghosts;
    [SerializeField] private int ghostChance;
    [SerializeField] private Target[] bunnies;
    [SerializeField] private int bunnyChance;
    [SerializeField] private Target[] deers;
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
        if (ghosts == null || ghosts.Length == 0)
        {
            Debug.LogError(name + " is missing ghosts");
            return;
        }
        if (image == null)
        {
            image = GetComponent<Image>();
            Debug.LogWarning(name + " is missing the reference to its image");
        }
        if (button == null)
        {
            button = GetComponent<Button>();
            Debug.LogWarning(name + " is missing the reference to its button");
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.LogWarning(name + " is missing the reference to its animator");
        }
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
        if (chance <= ghostChance)
            currentTarget = ghosts[Random.Range(0, ghosts.Length)];
        else if (chance > ghostChance && chance <= bunnyChance)
            currentTarget = bunnies[Random.Range(0, bunnies.Length)];
        else if (chance > bunnyChance)
            currentTarget = deers[Random.Range(0, deers.Length)];

        image.sprite = currentTarget.sprite;
        image.enabled = true;
        button.enabled = true;
        isActive = true;
        animator.SetInteger("Move", animIdx);
        disappearCoroutine = StartCoroutine(Disappear(true));
    }

    public void Shoot()
    {
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
}
