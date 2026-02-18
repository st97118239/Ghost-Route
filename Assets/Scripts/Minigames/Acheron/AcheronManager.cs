using UnityEngine;
using UnityEngine.UI;

public class AcheronManager : MonoBehaviour
{
    public Player player;

    [SerializeField] private Platform platforms;

    [SerializeField] private GameObject phantomPrefab;
    [SerializeField] private GameObject ferrymanPrefab;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartPos;
    [SerializeField] private float heartSpacing;
    [SerializeField] private int heartCount;
    private Image[] hearts;
    [SerializeField] private Sprite emptyHeartImage;

    public GameObject GetPhantomPrefab() => phantomPrefab;
    public GameObject GetFerrymanPrefab() => ferrymanPrefab;

    private void Awake()
    {
        hearts = new Image[heartCount];
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartPos.position + Vector3.right * (i * heartSpacing), Quaternion.identity, canvas.transform);
            hearts[i] = heart.GetComponent<Image>();
        }
    }

    public void Hit()
    {
        heartCount--;
        hearts[heartCount].sprite = emptyHeartImage;
        hearts[heartCount].gameObject.SetActive(false);

        if (heartCount <= 0)
            player.Death();
    }
}
