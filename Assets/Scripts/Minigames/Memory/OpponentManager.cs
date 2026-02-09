using System.Collections;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    // TODO:
    // Make opponent remember last couple cards so it won't do duplicate moves

    [SerializeField] private MemoryManager memoryManager;

    private CardObj[] seenCards;
    private int cardsSeen;

    [SerializeField] private int forgetCardsThreshold;
    [SerializeField] private int newCardChance;

    [SerializeField] private float _timeBetweenCards;
    private WaitForSeconds timeBetweenCards;

    private int idx0;
    private int idx1;

    private CardObj card0;
    private CardObj card1;

    public void Setup(int cardAmt)
    {
        seenCards = new CardObj[cardAmt];
        timeBetweenCards = new WaitForSeconds(_timeBetweenCards);
    }

    public void StartTurn()
    {
        if (memoryManager.cardsLeft == 0) return;

        idx0 = -1;
        idx1 = -1;
        card0 = null;
        card1 = null;

        if (cardsSeen < 4)
        {
            if (idx0 == -1)
                StartCoroutine(ChooseCards());
        }
        else
        {
            if (cardsSeen >= forgetCardsThreshold)
            {
                ForgetCards();
            }

            StartCoroutine(ChooseCards());
        }
    }

    private IEnumerator ChooseCards()
    {
        StartCoroutine(CheckForMatch());

        if (idx0 > -1)
            yield break;

        card0 = null;
        card1 = null;
        if (cardsSeen == 0)
            ChooseNewCard0();
        else if (memoryManager.cardsLeft > cardsSeen)
        {
            int chance = newCardChance + cardsSeen;
            int rnd = Random.Range(0, 100);

            if (chance >= rnd)
                ChooseNewCard0();
            else
                ChooseSeenCard0();
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
        else if (cardsSeen == 0)
            ChooseNewCard0();
        else if (memoryManager.cardsLeft > cardsSeen)
        {
            int chance = newCardChance + cardsSeen;
            int rnd = Random.Range(0, 100);

            if (chance >= rnd)
                ChooseNewCard1();
            else
                ChooseSeenCard1();
        }
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
        idx0 = idxA;
        card0 = memoryManager.PickCard(idx0);

        yield return timeBetweenCards;

        idx1 = idxB;
        card1 = memoryManager.PickCard(idx1);
    }

    private void ChooseNewCard0()
    {
        idx0 = -1;
        while (idx0 == -1)
        {
            int rnd = Random.Range(0, seenCards.Length);
            if (rnd != idx1 && seenCards[rnd] == null && memoryManager.IsCardAllowed(rnd))
                idx0 = rnd;
        }

        card0 = memoryManager.PickCard(idx0);
    }

    private void ChooseNewCard1()
    {
        idx1 = -1;
        while (idx1 == -1)
        {
            int rnd = Random.Range(0, seenCards.Length);
            if (rnd != idx0 && seenCards[rnd] == null && memoryManager.IsCardAllowed(rnd))
                idx1 = rnd;
        }

        card1 = memoryManager.PickCard(idx1);
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
        bool finished = false;

        while (!finished)
        {
            int rnd = Random.Range(0, seenCards.Length);
            if (seenCards[rnd] == null) continue;
            seenCards[rnd] = null;
            cardsSeen--;
            finished = true;
        }
    }

    public void SeeCard(CardObj card, int idx)
    {
        if (seenCards[idx] != null) return;
        seenCards[idx] = card;
        cardsSeen++;
    }

    public void ForgetCards(int idxA, int idxB)
    {
        seenCards[idxA] = null;
        seenCards[idxB] = null;
        cardsSeen -= 2;
    }
}
