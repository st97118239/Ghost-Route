using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TargetObj : MonoBehaviour
{
    private GhostHuntManager manager;

    [SerializeField] private Target[] targets;
    private Target currentTarget;

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
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError(name + " is missing targets");
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
    }

    public void Setup(GhostHuntManager givenManager, int givenIdx)
    {
        manager = givenManager;
        idx = givenIdx;
    }

    public void Show()
    {
        currentTarget = targets[Random.Range(0, targets.Length)];
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
}
