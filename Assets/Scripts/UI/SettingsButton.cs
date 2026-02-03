using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button.ButtonClickedEvent onEnter;
    public Button.ButtonClickedEvent onExit;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Image image;
    [SerializeField] private float fadeTime;

    private Coroutine coroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Fade(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Fade(true));
    }

    private IEnumerator Fade(bool shouldReverse)
    {
        Color endColor = shouldReverse ? defaultColor : selectColor;

        for (float i = 0; i <= fadeTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            Color imageColor = Color.Lerp(image.color, endColor, fillAmount);

            image.color = imageColor;

            yield return null;
        }
    }
}
