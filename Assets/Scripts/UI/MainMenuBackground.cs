using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image backgroundToFade;
    private const float fadeTime = 0.7f;
    private Coroutine backgroundFadeCoroutine;

    private readonly Color invisibleColor = new(1, 1, 1, 0);

    public void ChangeBackground(bool hasMoon)
    {
        if (backgroundFadeCoroutine != null)
            StopCoroutine(backgroundFadeCoroutine);
        backgroundFadeCoroutine = StartCoroutine(FadeOutBackground(!hasMoon));
    }

    private IEnumerator FadeOutBackground(bool fadeOut)
    {
        Color startColor;
        if (backgroundToFade.color.a is 0 or >= 255)
            startColor = fadeOut ? Color.white : invisibleColor;
        else
            startColor = backgroundToFade.color;
        Color endColor = fadeOut ? invisibleColor : Color.white;

        backgroundToFade.color = startColor;

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            backgroundToFade.color = Color.Lerp(startColor, endColor, fillAmount);

            yield return null;
        }

        backgroundToFade.color = endColor;
    }
}
