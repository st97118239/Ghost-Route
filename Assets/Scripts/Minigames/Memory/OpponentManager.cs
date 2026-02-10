using System.Collections;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    [SerializeField] private MemoryManager memoryManager;

    private CardObj[] seenCards;
    private int[] seenCardsMemory;
    private int cardsSeen;

    [SerializeField] private int[] cardMemoryAmt;
    [SerializeField] private int forgetCardChance;
    [SerializeField] private int chanceToForgetDouble;
    [SerializeField] private int newCardChance;
    [SerializeField] private int chanceToScore;
    [SerializeField] private int thresholdToNotPlayPrevTurn;

    [SerializeField] private float _timeBetweenCards;
    private WaitForSeconds timeBetweenCards;

    private int idx0;
    private int idx1;

    private CardObj card0;
    private CardObj card1;

    public void Setup(int cardAmt)
    {
        if (cardMemoryAmt.Length > 2)
        {
            int[] temp = { cardMemoryAmt[0], cardMemoryAmt[1] };
            cardMemoryAmt = temp;
        }
        seenCards = new CardObj[cardAmt];
        seenCardsMemory = new int[cardAmt];
        for (int i = 0; i < seenCardsMemory.Length; i++)
        {
            seenCardsMemory[i] = -1;
        }
        timeBetweenCards = new WaitForSeconds(_timeBetweenCards);
    }

    public void NewTurn(bool isOpponentTurn)
    {
        if (Random.Range(0, 100) <= chanceToForgetDouble)
            ForgetCards();
        ForgetCards();

        if (isOpponentTurn)
            StartTurn();
    }

    public void StartTurn()
    {
        if (memoryManager.cardsLeft == 0) return;

        idx0 = -1;
        idx1 = -1;
        card0 = null;
        card1 = null;

        StartCoroutine(ChooseCards());
    }

    private IEnumerator ChooseCards()
    {
        StartCoroutine(CheckForMatch());

        if (idx0 > -1)
            yield break;

        card0 = null;
        card1 = null;
        if (cardsSeen == 0)
            ChooseNewCard0(-1);
        else if (memoryManager.cardsLeft > cardsSeen)
        {
            int chance = newCardChance + cardsSeen;
            int rnd = Random.Range(0, 100);

            if (chance >= rnd)
                ChooseNewCard0(-1);
            else
                ChooseSeenCard0();
        }
        else
        {
            Debug.LogError("No card found.");
            yield break;
        }

        yield return timeBetweenCards;

        int idx = -1;
        for (int i = 0; i < seenCards.Length; i++)
        {
            if (i == idx0 || seenCards[i] == null) continue;
            Cards card = seenCards[i].card;
            if (card == card0.card)
                idx = i;
        }

        if (idx > -1)
        {
            idx1 = idx;
            card1 = memoryManager.PickCard(idx1);
        }
        else if (cardsSeen <= 1)
            ChooseNewCard1(-1);
        else if (memoryManager.cardsLeft > cardsSeen)
        {
            int chance = newCardChance + cardsSeen;
            int rnd = Random.Range(0, 100);

            if (chance >= rnd)
                ChooseNewCard1(-1);
            else
                ChooseSeenCard1();
        }
        else
            Debug.LogError("No card found.");
    }

    private IEnumerator CheckForMatch()
    {
        int idxA = -1;
        int idxB = -1;
        for (int i = 0; i < seenCards.Length; i++)
        {
            if (seenCards[i] == null) continue;
            Cards card = seenCards[i].card;

            for (int j = 0; j < seenCards.Length; j++)
            {
                if (i == j || seenCards[j] == null) continue;
                Cards otherCard = seenCards[j].card;

                if (card != otherCard) continue;
                idxA = i;
                idxB = j;
                break;
            }

            if (idxA != -1)
                break;
        }

        if (idxA == -1) yield break;

        if (Random.Range(0, 100) > chanceToScore) yield break;

        idx0 = idxA;
        card0 = memoryManager.PickCard(idx0);

        yield return timeBetweenCards;

        idx1 = idxB;
        card1 = memoryManager.PickCard(idx1);
    }

    private void ChooseNewCard0(int prevIdx)
    {
        while (true)
        {
            idx0 = -1;
            while (idx0 == -1)
            {
                int rnd = Random.Range(0, seenCards.Length);
                if (rnd != idx1 && seenCards[rnd] == null && memoryManager.IsCardAllowed(rnd)) idx0 = rnd;
            }

            if (idx0 != prevIdx && seenCardsMemory[idx0] > thresholdToNotPlayPrevTurn)
            {
                prevIdx = idx0;
                continue;
            }

            card0 = memoryManager.PickCard(idx0);
            break;
        }
    }

    private void ChooseNewCard1(int prevIdx)
    {
        while (true)
        {
            idx1 = -1;
            while (idx1 == -1)
            {
                int rnd = Random.Range(0, seenCards.Length);
                if (rnd != idx0 && seenCards[rnd] == null && memoryManager.IsCardAllowed(rnd)) idx1 = rnd;
            }

            if (idx1 != prevIdx && seenCardsMemory[idx1] > thresholdToNotPlayPrevTurn)
            {
                prevIdx = idx1;
                continue;
            }

            card1 = memoryManager.PickCard(idx1);
            break;
        }
    }

    private void ChooseSeenCard0()
    {
        idx0 = -1;
        while (idx0 == -1)
        {
            int rnd = Random.Range(0, seenCards.Length);
            if (rnd != idx1 && seenCards[rnd] != null && memoryManager.IsCardAllowed(rnd))
                idx0 = rnd;
        }

        card0 = memoryManager.PickCard(idx0);
    }

    private void ChooseSeenCard1()
    {
        idx1 = -1;
        while (idx1 == -1)
        {
            int rnd = Random.Range(0, seenCards.Length);
            if (rnd != idx0 && seenCards[rnd] != null && memoryManager.IsCardAllowed(rnd))
                idx1 = rnd;
        }

        card1 = memoryManager.PickCard(idx1);
    }

    private void ForgetCards()
    {
        for (int i = 0; i < seenCards.Length; i++)
        {
            if (seenCards[i] == null) continue;

            seenCardsMemory[i]--;

            if (seenCardsMemory[i] != 0) continue;
            int rnd = Random.Range(0, 100);
            if (rnd <= forgetCardChance) 
                ForgetCard(i);
            else
                seenCardsMemory[i] = Random.Range(1, cardMemoryAmt[1]);
        }
    }

    public void ForgetCard(int idx)
    {
        seenCards[idx] = null;
        seenCardsMemory[idx] = -1;
        cardsSeen--;
    }

    public void SeeCard(CardObj card, int idx)
    {
        if (seenCards[idx] != null) return;
        seenCards[idx] = card;
        seenCardsMemory[idx] = Random.Range(cardMemoryAmt[0], cardMemoryAmt[1]);
        cardsSeen++;
    }
}
