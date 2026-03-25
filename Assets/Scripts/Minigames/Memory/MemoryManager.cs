using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class MemoryManager : MonoBehaviour
{
    [SerializeField] private OpponentManager opponentManager;

    [SerializeField] private GameObject clickBlocker;

    [SerializeField] private GridLayoutGroup cardsParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int cardAmt;
    [SerializeField] private CardObj[] cardsObj;
    [SerializeField] private Cards[] cards;

    public int cardsLeft { get; private set; }

    [SerializeField] private float _timeTillCheck;
    private WaitForSeconds timeTillCheck;

    [SerializeField] private RectTransform moveAnimPosPlayer;
    [SerializeField] private RectTransform moveAnimPosOpponent;
    [SerializeField] private float moveAnimSpeed;

    private int playerPoints;
    private int opponentPoints;

    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text opponentText;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction devInputAction;

    [SerializeField] private AudioClip[] beginVoicelines;

    [SerializeField] private float timeBetweenCardPopup;
    private WaitForSeconds timeBetweenCardPopupWait;

    private readonly Random rng = new();

    private int[] cardsIdx;
    private string[] cardsId;

    public int turn { get; private set; }

    private void Awake()
    {
        FadeManager.Show();
        turn = -1;
        timeTillCheck = new WaitForSeconds(_timeTillCheck);
        timeBetweenCardPopupWait = new WaitForSeconds(timeBetweenCardPopup);
        cardsIdx = new[] { -1, -1 };
        cardsId = new[] { string.Empty, string.Empty };
        GenerateCardsArray();
        cardsLeft = cardAmt;
        StartCoroutine(SpawnCards());
    }

    private void Start() => FadeManager.StartFade(true, StartVoicelines, Color.black);

    private void StartVoicelines()
    {
        AudioManager.PlaySound(Sounds.Music, false);
        StartCoroutine(PlayVoicelines());
    }

    private IEnumerator PlayVoicelines()
    {
        yield return new WaitForSeconds(1);

        if (beginVoicelines != null && beginVoicelines.Length > 0)
        {
            foreach (AudioClip voiceline in beginVoicelines)
            {
                float delay = AudioManager.PlayVoiceline(voiceline);

                yield return new WaitForSeconds(delay);
            }
        }

        StartCoroutine(EnableCards());
    }

    private void StartGame()
    {
        opponentManager.Setup(cardAmt);
        turn = 0;
        clickBlocker.SetActive(false);

        devInputAction = inputActionAsset.FindAction("Dev/Dev Input");
        devInputAction.performed += DevShowCards;
        devInputAction.Enable();
    }

    private void GenerateCardsArray()
    {
        Cards[] tempCards = new Cards[cards.Length * 2];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < cards.Length; j++)
            {
                Cards card = cards[j];
                int idx = j;
                if (i == 1)
                    idx += cards.Length;
                tempCards[idx] = card;
            }
        }

        cards = tempCards.OrderBy(_ => rng.Next()).ToArray();
    }

    private IEnumerator SpawnCards()
    {
        cardsParent.enabled = true;
        cardsObj = new CardObj[cardAmt];
        for (int i = 0; i < cardAmt; i++)
        {
            CardObj card = Instantiate(cardPrefab, cardsParent.transform).GetComponent<CardObj>();
            cardsObj[i] = card;
            card.Load(this, cards[i], i);
        }

        yield return new WaitForEndOfFrame();
        cardsParent.enabled = false;
    }

    private IEnumerator EnableCards()
    {
        foreach (CardObj card in cardsObj)
        {
            card.Show();
            yield return timeBetweenCardPopupWait;
        }

        StartGame();
    }
    
    public void OnCardClicked(CardObj clickedCard, int clickedCardIdx)
    {
        if (cardsIdx[0] == -1)
        {
            cardsId[0] = clickedCard.card.id;
            cardsIdx[0] = clickedCardIdx;
            opponentManager.SeeCard(clickedCard, clickedCardIdx);
        }
        else
        {
            cardsId[1] = clickedCard.card.id;
            cardsIdx[1] = clickedCardIdx;
            opponentManager.SeeCard(clickedCard, clickedCardIdx);
            StartCoroutine(CheckCards());
        }

        AudioManager.PlaySound(Sounds.Click, false);
    }

    private IEnumerator CheckCards()
    {
        clickBlocker.SetActive(true);
        yield return timeTillCheck;

        if (cardsId[0] == cardsId[1])
        {
            if (turn == 0)
            {
                playerPoints++;
                playerText.text = playerPoints.ToString();
            }
            else
            {
                opponentPoints++;
                opponentText.text = opponentPoints.ToString();
            }

            CardObj card0 = cardsObj[cardsIdx[0]];
            CardObj card1 = cardsObj[cardsIdx[1]];
            card0.isActive = false;
            card1.isActive = false;
            RectTransform card0Rect = cardsObj[cardsIdx[0]].GetComponent<RectTransform>();
            RectTransform card1Rect = cardsObj[cardsIdx[1]].GetComponent<RectTransform>();
            Vector3 targetPos;
            if (turn == 0)
            {
                targetPos = moveAnimPosPlayer.position;
                card0.transform.SetParent(moveAnimPosPlayer);
                card1.transform.SetParent(moveAnimPosPlayer);
                card0.PlayVoiceline(false);
            }
            else
            {
                targetPos = moveAnimPosOpponent.position;
                card0.transform.SetParent(moveAnimPosOpponent);
                card1.transform.SetParent(moveAnimPosOpponent);
                card0.PlayVoiceline(true);
            }

            AudioManager.PlaySound(Sounds.MemoryPoint, false);

            while (card0Rect.position != targetPos || card1Rect.position != targetPos)
            {
                card0Rect.position = Vector2.MoveTowards(card0Rect.position, targetPos, moveAnimSpeed);
                card1Rect.position = Vector2.MoveTowards(card1Rect.position, targetPos, moveAnimSpeed);

                yield return Time.deltaTime;
            }

            opponentManager.ForgetCard(cardsIdx[0]);
            opponentManager.ForgetCard(cardsIdx[1]);
            cardsLeft -= 2;
        }
        else
        {
            cardsObj[cardsIdx[0]].ResetCard();
            cardsObj[cardsIdx[1]].ResetCard();
            turn = turn == 0 ? 1 : 0;
        }

        cardsId[0] = string.Empty;
        cardsId[1] = string.Empty;
        cardsIdx[0] = -1;
        cardsIdx[1] = -1;

        if (cardsLeft == 0)
        {
            EndGame();
            yield break;
        }

        if (turn == 0)
        {
            opponentManager.NewTurn(false);
            clickBlocker.SetActive(false);
        }
        else
            opponentManager.NewTurn(true);
    }

    public CardObj PickCard(int idx)
    {
        cardsObj[idx].Press();
        return cardsObj[idx];
    }

    public bool IsCardAllowed(int idx) => cardsObj[idx].isActive;

    public void EndGame()
    {
        devInputAction.Disable();
        devInputAction.performed -= DevShowCards;
        AudioManager.PlaySound(Sounds.Ending, false);
        SaveDataManager.saveData.hasPlayedMemory = true;
        SaveDataManager.saveData.memoryScore = playerPoints;
        FadeManager.StartFade(false, ExitGame, Color.black);
    }

    private static void ExitGame() => SceneManager.LoadScene("Dialogue");

    private void DevShowCards(InputAction.CallbackContext context)
    {
        foreach (CardObj card in cardsObj)
        {
            if (card.isActive)
                card.DevShowCard();
        }

        devInputAction.Disable();
    }
}