using UnityEngine;
using UnityEngine.UI;

public class CardObj : MonoBehaviour
{
    public Cards card { get; private set; }
    private int idx;

    [SerializeField] private Animator animator;

    [SerializeField] private Image image;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite frontSprite;

    private bool isShowing;
    private bool isPressed;
    public bool isActive;
    private bool isDevCheatActive;

    private MemoryManager memoryManager;

    public void Load(MemoryManager givenManager, Cards givenCard, int givenIdx)
    {
        memoryManager = givenManager;
        card = givenCard;
        idx = givenIdx;
        isShowing = false;
        frontSprite = card.sprite;
        image.sprite = backSprite;
        image.color = Color.clear;
    }

    public void Show()
    {
        image.color = Color.white;
        isActive = true;
    }

    public void Press()
    {
        if (isPressed) return;
        isPressed = true;
        memoryManager.OnCardClicked(this, idx);
        animator.SetBool("Flip", true);
    }

    public void ResetCard()
    {
        animator.SetBool("Flip", !isShowing);
        isPressed = false;
    }

    public void SwitchImage()
    {
        if (isShowing)
        {
            if (!isDevCheatActive)
                image.sprite = backSprite;
            else
                image.color = Color.grey;
            isShowing = false;
        }
        else
        {
            if (!isDevCheatActive)
                image.sprite = frontSprite;
            else
                image.color = Color.white;
            isShowing = true;
        }
    }

    public void DevShowCard()
    {
        isDevCheatActive = true;
        image.sprite = frontSprite;
        image.color = isShowing ? Color.white : Color.grey;
    }

    public void PlayVoiceline(bool isOpponent)
    {
        AudioManager.PlayVoiceline(isOpponent ? card.opponentScoreVoiceline : card.playerScoreVoiceline);
    }
}