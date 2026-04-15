using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image moon;
    [SerializeField] private Image moonGlow;
    [SerializeField] private Image[] stars;
    [SerializeField] private Image companyLogo;
    [SerializeField] private Button companyLogoButton;

    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private MainMenuManager mainMenu;

    [SerializeField] private Color moonGlowStartColor;
    [SerializeField] private Color moonGlowEndColor;
    private Color moonGlowStartLoopColor;
    private Color moonGlowEndLoopColor;
    [SerializeField] private float moonGlowTime;
    [SerializeField] private float moonGlowTimeUntilLoop;
    private WaitForSeconds moonGlowTimeUntilLoopWait;
    private Coroutine moonGlowCoroutine;
    private bool isMoonGlowLooping;
    private float moonGlowLoopIndex;

    [SerializeField] private float fadeTime = 0.7f;
    private Coroutine backgroundFadeCoroutine;

    private int starsIdx;
    [SerializeField] private float starsFadeTime = 1.2f;
    [SerializeField] private float starsContinueDelay;
    private WaitForSeconds starsContinueDelayWait;

    public static readonly Color invisibleColor = new(1, 1, 1, 0);

    private void Awake()
    {
        moonGlowTimeUntilLoopWait = new WaitForSeconds(moonGlowTimeUntilLoop);
        starsContinueDelayWait = new WaitForSeconds(starsContinueDelay);
        StartCoroutine(FadeStars());
    }

    public void ShowCredits()
    {
        if (creditsScreen.activeSelf)
        {
            creditsScreen.SetActive(false);
            mainMenu.Show(false);
        }
        else
        {
            mainMenu.Hide();
            creditsScreen.SetActive(true);
        }
    }

    public void ChangeBackground(bool hasMoon)
    {
        if (backgroundFadeCoroutine != null)
            StopCoroutine(backgroundFadeCoroutine);
        backgroundFadeCoroutine = StartCoroutine(FadeBackground(!hasMoon));
    }

    private IEnumerator FadeBackground(bool fadeOut)
    {
        Color startMoonGlowColor = invisibleColor;
        Color endMoonGlowColor = invisibleColor;

        switch (fadeOut)
        {
            case false:
                moonGlowCoroutine = StartCoroutine(MoonGlow());
                break;
            case true:
                if (moonGlowCoroutine != null)
                    StopCoroutine(moonGlowCoroutine);
                startMoonGlowColor = moonGlow.color;
                endMoonGlowColor = invisibleColor;
                break;
        }

        Color startColor = moon.color;
        Color endColor = fadeOut ? invisibleColor : Color.white;

        Color startLogoColor = companyLogo.color;
        Color endLogoColor = fadeOut ? invisibleColor : Color.white;

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            moon.color = Color.Lerp(startColor, endColor, fillAmount);
            if (fadeOut) 
                moonGlow.color = Color.Lerp(startMoonGlowColor, endMoonGlowColor, fillAmount);
            companyLogo.color = Color.Lerp(startLogoColor, endLogoColor, fillAmount);

            yield return null;
        }

        moon.color = endColor;

        if (!fadeOut) yield break;
        StopCoroutine(moonGlowCoroutine);
        moonGlow.color = endMoonGlowColor;
    }

    private IEnumerator MoonGlow()
    {
        isMoonGlowLooping = false;
        moonGlowLoopIndex = 0;
        moonGlowStartLoopColor = invisibleColor;
        moonGlowEndLoopColor = moonGlowEndColor;
        while (true)
        {
            for (float i = moonGlowLoopIndex; i <= moonGlowTime + Time.deltaTime; i += Time.deltaTime)
            {
                if (i > moonGlowTime) i = moonGlowTime;

                float fillAmount = i / moonGlowTime;

                moonGlow.color = Color.Lerp(moonGlowStartLoopColor, moonGlowEndLoopColor, fillAmount);

                moonGlowLoopIndex = i;
                yield return null;
            }

            moonGlow.color = moonGlowEndLoopColor;

            yield return moonGlowTimeUntilLoopWait;

            isMoonGlowLooping = !isMoonGlowLooping;
            moonGlowStartLoopColor = isMoonGlowLooping ? moonGlowEndColor : moonGlowStartColor;
            moonGlowEndLoopColor = isMoonGlowLooping ? moonGlowStartColor : moonGlowEndColor;
            moonGlowLoopIndex = 0;
        }
    }

    private IEnumerator FadeStars()
    {
        while (true)
        {
            if (starsIdx > stars.Length)
                starsIdx = 0;
            Image starToFadeOut = stars[starsIdx];
            Image starToFadeIn;
            if (starsIdx >= stars.Length - 1)
            {
                starToFadeIn = stars[0];
                starsIdx++;
            }
            else
                starToFadeIn = stars[starsIdx + 1];

            starToFadeIn.color = invisibleColor;
            starToFadeOut.color = Color.white;

            for (float i = 0; i <= starsFadeTime + Time.deltaTime; i += Time.deltaTime)
            {
                if (i > starsFadeTime)
                    i = starsFadeTime;

                float fillAmount = i / starsFadeTime;

                starToFadeOut.color = Color.Lerp(Color.white, invisibleColor, fillAmount);
                starToFadeIn.color = Color.Lerp(invisibleColor, Color.white, fillAmount);

                yield return null;
            }

            starsIdx++;

            starToFadeIn.color = Color.white;
            starToFadeOut.color = invisibleColor;

            yield return starsContinueDelayWait;
        }
    }
}
