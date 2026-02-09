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

    private MemoryManager memoryManager;

    public void Load(MemoryManager givenManager, Cards givenCard, int givenIdx)
    {
        memoryManager = givenManager;
        card = givenCard;
        idx = givenIdx;
        isShowing = false;
        frontSprite = card.sprite;
        image.sprite = backSprite;
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
            image.sprite = backSprite;
            isShowing = false;
        }
        else
        {
            image.sprite = frontSprite;
            isShowing = true;
        }
    }
}