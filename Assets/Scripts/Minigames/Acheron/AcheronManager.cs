using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AcheronManager : MonoBehaviour
{
    public Player player;

    [SerializeField] private GameObject phantomPrefab;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartPos;
    [SerializeField] private float heartSpacing;
    [SerializeField] private int heartCount;
    private Image[] hearts;
    [SerializeField] private Sprite emptyHeartImage;

    private string sceneToGoTo;

    public GameObject GetPhantomPrefab() => phantomPrefab;

    private void Awake()
    {
        FadeManager.Show();
        hearts = new Image[heartCount];
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartPos.position + Vector3.right * (i * heartSpacing), Quaternion.identity, canvas.transform);
            hearts[i] = heart.GetComponent<Image>();
        }
    }

    private void Start()
    {
        AudioManager.PlaySound(Sounds.Music);
        FadeManager.StartFade(true, null);
    }

    public void Instakill()
    {
        AudioManager.PlaySound(Sounds.Fall);
        for (int i = 0; i < heartCount; i++) 
            Hit();
    }

    public void Hit()
    {
        heartCount--;
        hearts[heartCount].sprite = emptyHeartImage;

        if (heartCount <= 0)
            player.End(true, false);
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(retryButton);
    }

    public void RetryButton()
    {
        AudioManager.instance.PlayButtonClick();
        sceneToGoTo = "Acheron";
        FadeManager.StartFade(false, LoadScene);
    }

    public void Quit()
    {
        sceneToGoTo = "Main Menu";
        FadeManager.StartFade(false, LoadScene);
    }

    public void Finish()
    {
        SaveDataManager.saveData.hasPlayedAcheron = true;
        sceneToGoTo = "Dialogue";
        FadeManager.StartFade(false, LoadScene);
    }

    private void LoadScene() => SceneManager.LoadScene(sceneToGoTo);

    public void DevInvincible()
    {
        for (int i = 0; i < heartCount; i++)
        {
            hearts[i].color = Color.grey;
        }
    }
}
