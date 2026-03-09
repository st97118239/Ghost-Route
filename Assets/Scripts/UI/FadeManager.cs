using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    private Image fadeImage;
    private Canvas canvas;
    [SerializeField] private float fadeTime;

    private Coroutine fadeCoroutine;

    private void Reset()
    {
        ResetPanel();
        CreatePanel();
    }

    private void Awake()
    {
        instance = this;
        fadeImage = transform.Find("FadePanel").GetComponent<Image>();
        canvas = GetComponent<Canvas>();
        canvas.enabled = true;
    }

    private void ResetPanel()
    {
        DestroyImmediate(gameObject.GetComponent<GraphicRaycaster>());
        DestroyImmediate(gameObject.GetComponent<CanvasScaler>());
        if (!canvas)
            canvas = GetComponent<Canvas>();
        if (canvas)
            DestroyImmediate(canvas);
        Transform panel = transform.Find("FadePanel");
        if (panel != null)
            DestroyImmediate(panel.gameObject);
    }

    private void CreatePanel()
    {
        gameObject.name = "FadeManager";
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
        canvas.sortingOrder = 5;

        CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        GraphicRaycaster graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        GameObject panelObj = new("FadePanel");
        panelObj.transform.SetParent(transform);
        fadeImage = panelObj.AddComponent<Image>();
        fadeImage.sprite = null;
        fadeImage.color = Color.clear;
        panelObj.SetActive(false);
        RectTransform panelTransform = panelObj.GetComponent<RectTransform>();
        panelTransform.anchoredPosition = Vector2.zero;
        panelTransform.pivot = new Vector2(0.5f, 0.5f);
        panelTransform.anchorMax = new Vector2(1, 1);
        panelTransform.anchorMin = new Vector2(0, 0);
        panelTransform.sizeDelta = new Vector2(0, 0);

        canvas.enabled = false;

        fadeTime = 1.5f;
    }

    public static void StartFade(bool fadeIn, Action callback) => instance.StartFadeCoroutine(fadeIn, callback);

    public static void Show()
    {
        instance.fadeImage.color = Color.black;
        instance.fadeImage.enabled = true;
        instance.fadeImage.gameObject.SetActive(true);
    }

    private void StartFadeCoroutine(bool fadeIn, Action callback)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(fadeIn, callback));
    }

    private IEnumerator Fade(bool fadeIn, Action callback)
    {
        Color startColor = fadeIn ? Color.black : Color.clear;
        Color endColor = fadeIn ? Color.clear : Color.black;
        fadeImage.color = startColor;

        fadeImage.gameObject.SetActive(true);

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            fadeImage.color = Color.Lerp(startColor, endColor, fillAmount);

            yield return null;
        }

        fadeImage.color = endColor;
        if (fadeIn)
            fadeImage.gameObject.SetActive(false);

        callback?.Invoke();
    }
}
