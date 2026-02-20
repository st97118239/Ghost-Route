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

    public GameObject GetPhantomPrefab() => phantomPrefab;

    private void Awake()
    {
        hearts = new Image[heartCount];
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartPos.position + Vector3.right * (i * heartSpacing), Quaternion.identity, canvas.transform);
            hearts[i] = heart.GetComponent<Image>();
        }
    }

    public void Instakill()
    {
        for (int i = 0; i < heartCount; i++)
        {
            Hit();
        }
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
        SceneManager.LoadScene("Acheron");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Finish()
    {
        SaveData.hasPlayedAcheron = true;
        SceneManager.LoadScene("Dialogue");
    }
}
